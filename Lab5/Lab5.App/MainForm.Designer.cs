using System.Drawing;
using System.Windows.Forms;

namespace Lab5.App
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private TabControl tabs;
        private TabPage tabMain;
        private TabPage tabStep1;
        private TabPage tabStep2;
        private TabPage tabStep3;
        private TabPage tabStep4;

        // главная вкладка
        private Button btnConnect;
        private Button btnSave;
        private Button btnBuildAll;
        private Label lblIntro;

        // шаг 1
        private TextBox tbBaseLen, tbBaseWidth, tbBaseHeight;
        private Button btnStep1;

        // шаг 2
        private TextBox tbFinBottom, tbFinTop, tbFinH, tbFinW;
        private Button btnStep2;

        // шаг 3
        private TextBox tbSlotW, tbSlotH, tbSlotOffset;
        private Button btnStep3;

        // шаг 4
        private TextBox tbNotchR;
        private Button btnStep4;

        // статус
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        private void InitializeComponent()
        {
            this.tabs = new TabControl();
            this.tabMain = new TabPage("Главная");
            this.tabStep1 = new TabPage("Шаг 1 — Основание");
            this.tabStep2 = new TabPage("Шаг 2 — Гребень");
            this.tabStep3 = new TabPage("Шаг 3 — Паз");
            this.tabStep4 = new TabPage("Шаг 4 — R30");

            this.SuspendLayout();

            this.tabs.Dock = DockStyle.Fill;
            this.tabs.Controls.Add(this.tabMain);
            this.tabs.Controls.Add(this.tabStep1);
            this.tabs.Controls.Add(this.tabStep2);
            this.tabs.Controls.Add(this.tabStep3);
            this.tabs.Controls.Add(this.tabStep4);

            BuildMainTab();
            BuildStep1Tab();
            BuildStep2Tab();
            BuildStep3Tab();
            BuildStep4Tab();

            this.statusStrip = new StatusStrip { SizingGrip = false };
            this.lblStatus = new ToolStripStatusLabel(
                "Готово. Подключитесь к SolidWorks на вкладке «Главная».");
            this.statusStrip.Items.Add(this.lblStatus);

            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(760, 480);
            this.MinimumSize = new Size(700, 430);
            this.Text = "Лаб. №5 — 3D-модель детали (SolidWorks API)";
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.statusStrip);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BuildMainTab()
        {
            this.lblIntro = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(12),
                Text = "Эта вкладка управляет подключением к SolidWorks " +
                       "и сохранением результата.\r\nПереключайтесь на " +
                       "«Шаг 1 … 4», задавайте параметры и постройте деталь " +
                       "пошагово, либо нажмите «Построить всё» здесь.",
            };

            this.btnConnect = new Button
            {
                Text = "Подключиться к SolidWorks",
                Width = 220,
                Height = 36,
                Top = 100,
                Left = 16,
            };
            this.btnConnect.Click += this.btnConnect_Click;

            this.btnBuildAll = new Button
            {
                Text = "Построить всё (шаги 1–4)",
                Width = 220,
                Height = 36,
                Top = 100,
                Left = 250,
            };
            this.btnBuildAll.Click += this.btnBuildAll_Click;

            this.btnSave = new Button
            {
                Text = "Сохранить как…",
                Width = 220,
                Height = 36,
                Top = 150,
                Left = 16,
            };
            this.btnSave.Click += this.btnSave_Click;

            this.tabMain.Controls.Add(this.lblIntro);
            this.tabMain.Controls.Add(this.btnConnect);
            this.tabMain.Controls.Add(this.btnBuildAll);
            this.tabMain.Controls.Add(this.btnSave);
        }

        private void BuildStep1Tab()
        {
            this.tbBaseLen = NewTextBox();
            this.tbBaseWidth = NewTextBox();
            this.tbBaseHeight = NewTextBox();

            this.btnStep1 = new Button
            {
                Text = "Построить основание",
                Width = 220,
                Height = 32,
            };
            this.btnStep1.Click += this.btnStep1_Click;

            var grid = BuildParamsGrid(
                ("Длина (мм):", this.tbBaseLen),
                ("Глубина (мм):", this.tbBaseWidth),
                ("Высота (мм):", this.tbBaseHeight));
            this.tabStep1.Controls.Add(grid);
            this.tabStep1.Controls.Add(WithBottomDock(this.btnStep1));
        }

        private void BuildStep2Tab()
        {
            this.tbFinBottom = NewTextBox();
            this.tbFinTop = NewTextBox();
            this.tbFinH = NewTextBox();
            this.tbFinW = NewTextBox();

            this.btnStep2 = new Button
            {
                Text = "Построить гребень",
                Width = 220,
                Height = 32,
            };
            this.btnStep2.Click += this.btnStep2_Click;

            var grid = BuildParamsGrid(
                ("Нижняя длина (мм):", this.tbFinBottom),
                ("Верхняя длина (мм):", this.tbFinTop),
                ("Высота (мм):", this.tbFinH),
                ("Глубина (мм):", this.tbFinW));
            this.tabStep2.Controls.Add(grid);
            this.tabStep2.Controls.Add(WithBottomDock(this.btnStep2));
        }

        private void BuildStep3Tab()
        {
            this.tbSlotW = NewTextBox();
            this.tbSlotH = NewTextBox();
            this.tbSlotOffset = NewTextBox();

            this.btnStep3 = new Button
            {
                Text = "Построить паз",
                Width = 220,
                Height = 32,
            };
            this.btnStep3.Click += this.btnStep3_Click;

            var grid = BuildParamsGrid(
                ("Ширина паза (мм):", this.tbSlotW),
                ("Высота паза (мм):", this.tbSlotH),
                ("Смещение по X (мм):", this.tbSlotOffset));
            this.tabStep3.Controls.Add(grid);
            this.tabStep3.Controls.Add(WithBottomDock(this.btnStep3));
        }

        private void BuildStep4Tab()
        {
            this.tbNotchR = NewTextBox();

            this.btnStep4 = new Button
            {
                Text = "Построить R30",
                Width = 220,
                Height = 32,
            };
            this.btnStep4.Click += this.btnStep4_Click;

            var grid = BuildParamsGrid(
                ("Радиус R30 (мм):", this.tbNotchR));
            this.tabStep4.Controls.Add(grid);
            this.tabStep4.Controls.Add(WithBottomDock(this.btnStep4));
        }

        private static TextBox NewTextBox()
        {
            return new TextBox { Dock = DockStyle.Fill, Margin = new Padding(0, 2, 12, 2) };
        }

        private static Panel WithBottomDock(Control inner)
        {
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(12) };
            inner.Left = 12;
            inner.Top = 8;
            panel.Controls.Add(inner);
            return panel;
        }

        private static TableLayoutPanel BuildParamsGrid(params (string label, TextBox box)[] rows)
        {
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = rows.Length + 1,
                Padding = new Padding(12),
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < rows.Length; i++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                grid.Controls.Add(new Label
                {
                    Text = rows[i].label,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                }, 0, i);
                grid.Controls.Add(rows[i].box, 1, i);
            }
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            return grid;
        }
    }
}
