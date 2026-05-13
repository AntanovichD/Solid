using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksApi
{
    /// <summary>
    /// Управляет жизненным циклом подключения к SolidWorks: запускает приложение
    /// (или присоединяется к запущенному), создаёт активный документ детали,
    /// предоставляет ссылки на <see cref="ISldWorks"/>, <see cref="IModelDoc2"/>,
    /// <see cref="ISketchManager"/> и <see cref="IFeatureManager"/>.
    /// </summary>
    public sealed class SolidWorksHost : IDisposable
    {
        private const string ProgId = "SldWorks.Application";

        public ISldWorks SwApp { get; private set; }
        public IModelDoc2 ModelDoc { get; private set; }
        public ISketchManager SketchManager => ModelDoc?.SketchManager;
        public IFeatureManager FeatureManager => ModelDoc?.FeatureManager;
        public IModelDocExtension Extension => ModelDoc?.Extension;

        public bool IsConnected => SwApp != null;

        /// <summary>
        /// Запускает или присоединяется к экземпляру SolidWorks. Делает окно
        /// видимым, чтобы студент сразу видел построение.
        /// </summary>
        public void Connect()
        {
            if (SwApp != null) return;

            var type = Type.GetTypeFromProgID(ProgId)
                       ?? throw new InvalidOperationException(
                           "SolidWorks не найден в реестре COM. Установлен ли SolidWorks?");

            SwApp = (ISldWorks)Activator.CreateInstance(type);
            SwApp.Visible = true;
            SwApp.UserControl = true;
        }

        /// <summary>
        /// Создаёт новый документ детали. Сначала пытается взять шаблон из настроек
        /// SolidWorks; если пусто или не найдено — сканирует стандартные папки.
        /// </summary>
        public IModelDoc2 NewPart()
        {
            if (SwApp == null) Connect();

            var attempts = new List<string>();

            string template = SwApp.GetUserPreferenceStringValue(
                (int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            if (!string.IsNullOrEmpty(template))
            {
                attempts.Add(template);
                ModelDoc = TryCreate(template);
                if (ModelDoc != null) return ModelDoc;
            }

            foreach (string candidate in EnumerateFallbackTemplates())
            {
                attempts.Add(candidate);
                ModelDoc = TryCreate(candidate);
                if (ModelDoc != null) return ModelDoc;
            }

            throw new InvalidOperationException(
                "Не удалось создать новую деталь. Пробовал шаблоны:\n  " +
                string.Join("\n  ", attempts.Count == 0
                    ? new[] { "(шаблон по умолчанию не задан)" }
                    : attempts.ToArray()) +
                "\nПроверьте: Сервис → Параметры → Свойства файлов → Расположение файлов → Шаблоны документов.");
        }

        private IModelDoc2 TryCreate(string templatePath)
        {
            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                return null;
            return SwApp.NewDocument(templatePath, 0, 0, 0) as IModelDoc2;
        }

        /// <summary>
        /// Перебирает стандартные расположения шаблонов SolidWorks. Сначала возвращает
        /// явные "Part.prtdot"/"Деталь.prtdot", затем — любые *.prtdot (например
        /// gost-part.prtdot на локализованных сборках).
        /// </summary>
        private static IEnumerable<string> EnumerateFallbackTemplates()
        {
            string[] roots =
            {
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            };
            string[] preferred =
            {
                "Part.prtdot", "Деталь.prtdot", "part.prtdot",
            };

            var all = new List<string>();
            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root) || !Directory.Exists(root)) continue;
                try
                {
                    all.AddRange(Directory.EnumerateFiles(
                        root, "*.prtdot", SearchOption.AllDirectories));
                }
                catch { /* пропускаем недоступные папки */ }
            }

            // сначала явные имена — быстрее срабатывают на стандартных установках
            foreach (string file in all)
            {
                string name = Path.GetFileName(file);
                foreach (string wanted in preferred)
                {
                    if (string.Equals(name, wanted, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return file;
                        break;
                    }
                }
            }
            // затем — любой прочий .prtdot (gost-part, custom, ит.п.)
            foreach (string file in all)
            {
                string name = Path.GetFileName(file);
                bool isPreferred = false;
                foreach (string wanted in preferred)
                    if (string.Equals(name, wanted, StringComparison.OrdinalIgnoreCase))
                    { isPreferred = true; break; }
                if (!isPreferred) yield return file;
            }
        }

        /// <summary>
        /// Выбирает плоскость по имени и переводит документ в режим эскиза.
        /// </summary>
        public void StartSketchOn(string planeName)
        {
            if (ModelDoc == null)
                throw new InvalidOperationException("Сначала создайте деталь (NewPart).");

            bool selected = Extension.SelectByID2(
                planeName, "PLANE", 0, 0, 0, false, 0, null, 0);
            if (!selected)
                throw new InvalidOperationException($"Плоскость \"{planeName}\" не найдена.");

            SketchManager.InsertSketch(true);
        }

        public void ExitSketch() => SketchManager.InsertSketch(true);

        public void ClearSelection() => ModelDoc.ClearSelection2(true);

        /// <summary>
        /// Выделяет все эскизные элементы и удаляет их ("Очистить"). Изолирует
        /// от UI работу с константами SolidWorks.
        /// </summary>
        public void ClearSketch()
        {
            if (ModelDoc == null) return;
            ModelDoc.ClearSelection2(true);
            ModelDoc.Extension.SelectAll();
            ModelDoc.Extension.DeleteSelection2(
                (int)swDeleteSelectionOptions_e.swDelete_Absorbed);
        }

        public void Rebuild()
        {
            ModelDoc.EditRebuild3();
            ModelDoc.ViewZoomtofit2();
        }

        /// <summary>
        /// Сохраняет текущую деталь как файл (для SaveFileDialog в UI).
        /// </summary>
        public void SaveAs(string filePath)
        {
            if (ModelDoc == null) throw new InvalidOperationException("Документ не открыт.");
            int errors = 0, warnings = 0;
            ModelDoc.Extension.SaveAs(
                filePath,
                (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
                null, ref errors, ref warnings);
            if (errors != 0)
                throw new InvalidOperationException(
                    $"Сохранение не удалось (errors = {errors}, warnings = {warnings}).");
        }

        public void Dispose()
        {
            try
            {
                if (ModelDoc != null) Marshal.ReleaseComObject(ModelDoc);
            }
            catch { /* при выходе игнорируем */ }
            try
            {
                if (SwApp != null) Marshal.ReleaseComObject(SwApp);
            }
            catch { /* при выходе игнорируем */ }

            ModelDoc = null;
            SwApp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
