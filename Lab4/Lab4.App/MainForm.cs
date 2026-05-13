using System;
using System.Globalization;
using System.Windows.Forms;
using SolidWorksApi;

namespace Lab4.App
{
    public partial class MainForm : Form
    {
        private readonly SolidWorksHost _host = new SolidWorksHost();

        public MainForm()
        {
            InitializeComponent();
            FillDefaults();
            FormClosing += (s, e) => _host.Dispose();
        }

        private void FillDefaults()
        {
            var p = new Lab4Parameters();
            tbOuterR.Text = p.OuterArcRadius.ToString(CultureInfo.InvariantCulture);
            tbInnerR.Text = p.InnerArcRadius.ToString(CultureInfo.InvariantCulture);
            tbSlot.Text = p.SlotHeight.ToString(CultureInfo.InvariantCulture);
            tbBottomH.Text = p.BottomHeight.ToString(CultureInfo.InvariantCulture);
            tbTopH.Text = p.TopHeight.ToString(CultureInfo.InvariantCulture);
            tbTotalH.Text = p.TotalHeight.ToString(CultureInfo.InvariantCulture);
            tbTopLeftW.Text = p.TopLeftWidth.ToString(CultureInfo.InvariantCulture);
            tbTopRightW.Text = p.TopRightWidth.ToString(CultureInfo.InvariantCulture);
            tbBottomTotalW.Text = p.BottomTotalWidth.ToString(CultureInfo.InvariantCulture);
            tbHoleDia.Text = p.HoleDiameter.ToString(CultureInfo.InvariantCulture);
        }

        private Lab4Parameters ReadParameters()
        {
            return new Lab4Parameters
            {
                OuterArcRadius = ParseMm(tbOuterR.Text, "Внешний радиус (R20)"),
                InnerArcRadius = ParseMm(tbInnerR.Text, "Внутренний радиус (R10)"),
                SlotHeight = ParseMm(tbSlot.Text, "Высота прорези"),
                BottomHeight = ParseMm(tbBottomH.Text, "Высота нижней части"),
                TopHeight = ParseMm(tbTopH.Text, "Высота от центра отверстия"),
                TotalHeight = ParseMm(tbTotalH.Text, "Полная высота"),
                TopLeftWidth = ParseMm(tbTopLeftW.Text, "Ширина левой части верха"),
                TopRightWidth = ParseMm(tbTopRightW.Text, "Ширина правой части верха"),
                BottomTotalWidth = ParseMm(tbBottomTotalW.Text, "Полная ширина опоры"),
                HoleDiameter = ParseMm(tbHoleDia.Text, "Диаметр отверстия")
            };
        }

        private static double ParseMm(string value, string fieldName)
        {
            if (!double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double result) || result <= 0)
                throw new FormatException(
                    $"Поле \"{fieldName}\" должно быть положительным числом.");
            return result;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _host.Connect();
                _host.NewPart();
                lblStatus.Text = "Подключено. Открыта новая деталь.";
            }
            catch (Exception ex)
            {
                ShowError("Не удалось подключиться к SolidWorks", ex);
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_host.IsConnected) _host.Connect();
                if (_host.ModelDoc == null) _host.NewPart();

                Lab4PartBuilder.Build(_host, ReadParameters());
                lblStatus.Text = "Эскиз построен.";
            }
            catch (Exception ex)
            {
                ShowError("Не удалось построить эскиз", ex);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (_host.ModelDoc == null) return;
                _host.ModelDoc.ClearSelection2(true);
                _host.ModelDoc.Extension.SelectAll();
                _host.ModelDoc.Extension.DeleteSelection2(
                    (int)SolidWorks.Interop.swconst.swDeleteSelectionOptions_e
                        .swDelete_Absorbed);
                lblStatus.Text = "Эскиз очищен.";
            }
            catch (Exception ex)
            {
                ShowError("Не удалось очистить эскиз", ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new SaveFileDialog
                {
                    Title = "Сохранить деталь",
                    Filter = "SolidWorks Part (*.SLDPRT)|*.SLDPRT|" +
                             "STEP AP214 (*.step;*.stp)|*.step;*.stp|" +
                             "Все файлы (*.*)|*.*",
                    DefaultExt = "SLDPRT",
                    FileName = "Lab4_Part"
                })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    _host.SaveAs(dlg.FileName);
                    lblStatus.Text = "Файл сохранён: " + dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                ShowError("Не удалось сохранить файл", ex);
            }
        }

        private void ShowError(string title, Exception ex)
        {
            lblStatus.Text = title + ": " + ex.Message;
            MessageBox.Show(
                this,
                ex.Message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
