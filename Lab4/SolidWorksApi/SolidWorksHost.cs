using System;
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
        /// Создаёт новый документ детали из шаблона по умолчанию.
        /// </summary>
        public IModelDoc2 NewPart()
        {
            if (SwApp == null) Connect();

            string template = SwApp.GetUserPreferenceStringValue(
                (int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

            ModelDoc = (IModelDoc2)SwApp.NewDocument(template, 0, 0, 0);
            if (ModelDoc == null)
                throw new InvalidOperationException(
                    "Не удалось создать новую деталь — проверьте шаблоны SolidWorks.");

            return ModelDoc;
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
