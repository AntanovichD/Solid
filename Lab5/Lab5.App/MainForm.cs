using System;
using System.Globalization;
using System.Windows.Forms;
using SolidWorksApi;

namespace Lab5.App
{
    public partial class MainForm : Form
    {
        private readonly SolidWorksHost _host = new SolidWorksHost();
        private Lab5PartBuilder _builder;
        private readonly Lab5Parameters _params = new Lab5Parameters();

        public MainForm()
        {
            InitializeComponent();
            FillDefaults();
            FormClosing += (s, e) => _host.Dispose();
        }

        private Lab5PartBuilder Builder
        {
            get
            {
                if (_builder == null) _builder = new Lab5PartBuilder(_host);
                return _builder;
            }
        }

        private void FillDefaults()
        {
            tbBaseLen.Text = _params.BaseLength.ToString(CultureInfo.InvariantCulture);
            tbBaseWidth.Text = _params.BaseWidth.ToString(CultureInfo.InvariantCulture);
            tbBaseHeight.Text = _params.BaseHeight.ToString(CultureInfo.InvariantCulture);

            tbFinBottom.Text = _params.FinBottomLength.ToString(CultureInfo.InvariantCulture);
            tbFinTop.Text = _params.FinTopLength.ToString(CultureInfo.InvariantCulture);
            tbFinH.Text = _params.FinHeight.ToString(CultureInfo.InvariantCulture);
            tbFinW.Text = _params.FinWidth.ToString(CultureInfo.InvariantCulture);

            tbSlotW.Text = _params.SlotWidth.ToString(CultureInfo.InvariantCulture);
            tbSlotH.Text = _params.SlotHeight.ToString(CultureInfo.InvariantCulture);
            tbSlotOffset.Text = _params.SlotOffsetX.ToString(CultureInfo.InvariantCulture);

            tbNotchR.Text = _params.NotchRadius.ToString(CultureInfo.InvariantCulture);
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

        private void ApplyStep1FromUi()
        {
            _params.BaseLength = ParseMm(tbBaseLen.Text, "Длина основания");
            _params.BaseWidth = ParseMm(tbBaseWidth.Text, "Глубина основания");
            _params.BaseHeight = ParseMm(tbBaseHeight.Text, "Высота основания");
        }

        private void ApplyStep2FromUi()
        {
            _params.FinBottomLength = ParseMm(tbFinBottom.Text, "Нижняя длина гребня");
            _params.FinTopLength = ParseMm(tbFinTop.Text, "Верхняя длина гребня");
            _params.FinHeight = ParseMm(tbFinH.Text, "Высота гребня");
            _params.FinWidth = ParseMm(tbFinW.Text, "Глубина гребня");
        }

        private void ApplyStep3FromUi()
        {
            _params.SlotWidth = ParseMm(tbSlotW.Text, "Ширина паза");
            _params.SlotHeight = ParseMm(tbSlotH.Text, "Высота паза");
            if (!double.TryParse(
                    tbSlotOffset.Text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double offset))
                throw new FormatException("Смещение паза должно быть числом.");
            _params.SlotOffsetX = offset;
        }

        private void ApplyStep4FromUi()
        {
            _params.NotchRadius = ParseMm(tbNotchR.Text, "Радиус выреза R30");
        }

        private void EnsureConnected()
        {
            if (!_host.IsConnected) _host.Connect();
            if (_host.ModelDoc == null) _host.NewPart();
        }

        // ---- кнопки «Главная» ----
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureConnected();
                lblStatus.Text = "Подключено. Открыта новая деталь.";
            }
            catch (Exception ex) { ShowError("Подключение", ex); }
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
                    FileName = "Lab5_Part"
                })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    _host.SaveAs(dlg.FileName);
                    lblStatus.Text = "Сохранено: " + dlg.FileName;
                }
            }
            catch (Exception ex) { ShowError("Сохранение", ex); }
        }

        private void btnBuildAll_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureConnected();
                ApplyStep1FromUi();
                ApplyStep2FromUi();
                ApplyStep3FromUi();
                ApplyStep4FromUi();
                Builder.BuildAll(_params);
                lblStatus.Text = "Все шаги выполнены.";
            }
            catch (Exception ex) { ShowError("Полное построение", ex); }
        }

        // ---- кнопки шагов ----
        private void btnStep1_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureConnected();
                ApplyStep1FromUi();
                Builder.BuildStep1(_params);
                lblStatus.Text = "Шаг 1 (основание) построен.";
            }
            catch (Exception ex) { ShowError("Шаг 1", ex); }
        }

        private void btnStep2_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureConnected();
                ApplyStep2FromUi();
                Builder.BuildStep2(_params);
                lblStatus.Text = "Шаг 2 (гребень) построен.";
            }
            catch (Exception ex) { ShowError("Шаг 2", ex); }
        }

        private void btnStep3_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureConnected();
                ApplyStep3FromUi();
                Builder.BuildStep3(_params);
                lblStatus.Text = "Шаг 3 (паз) построен.";
            }
            catch (Exception ex) { ShowError("Шаг 3", ex); }
        }

        private void btnStep4_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureConnected();
                ApplyStep4FromUi();
                Builder.BuildStep4(_params);
                lblStatus.Text = "Шаг 4 (R30) построен.";
            }
            catch (Exception ex) { ShowError("Шаг 4", ex); }
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
