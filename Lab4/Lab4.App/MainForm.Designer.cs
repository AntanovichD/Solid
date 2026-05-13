namespace Lab4.App
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

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.GroupBox gbParams;
        private System.Windows.Forms.TableLayoutPanel paramsLayout;
        private System.Windows.Forms.Label lblOuterR;
        private System.Windows.Forms.Label lblInnerR;
        private System.Windows.Forms.Label lblSlot;
        private System.Windows.Forms.Label lblBottomH;
        private System.Windows.Forms.Label lblTopH;
        private System.Windows.Forms.Label lblTotalH;
        private System.Windows.Forms.Label lblTopLeftW;
        private System.Windows.Forms.Label lblTopRightW;
        private System.Windows.Forms.Label lblBottomTotalW;
        private System.Windows.Forms.Label lblHoleDia;
        private System.Windows.Forms.TextBox tbOuterR;
        private System.Windows.Forms.TextBox tbInnerR;
        private System.Windows.Forms.TextBox tbSlot;
        private System.Windows.Forms.TextBox tbBottomH;
        private System.Windows.Forms.TextBox tbTopH;
        private System.Windows.Forms.TextBox tbTotalH;
        private System.Windows.Forms.TextBox tbTopLeftW;
        private System.Windows.Forms.TextBox tbTopRightW;
        private System.Windows.Forms.TextBox tbBottomTotalW;
        private System.Windows.Forms.TextBox tbHoleDia;
        private System.Windows.Forms.FlowLayoutPanel btnPanel;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

        private void InitializeComponent()
        {
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.gbParams = new System.Windows.Forms.GroupBox();
            this.paramsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblOuterR = new System.Windows.Forms.Label();
            this.lblInnerR = new System.Windows.Forms.Label();
            this.lblSlot = new System.Windows.Forms.Label();
            this.lblBottomH = new System.Windows.Forms.Label();
            this.lblTopH = new System.Windows.Forms.Label();
            this.lblTotalH = new System.Windows.Forms.Label();
            this.lblTopLeftW = new System.Windows.Forms.Label();
            this.lblTopRightW = new System.Windows.Forms.Label();
            this.lblBottomTotalW = new System.Windows.Forms.Label();
            this.lblHoleDia = new System.Windows.Forms.Label();
            this.tbOuterR = new System.Windows.Forms.TextBox();
            this.tbInnerR = new System.Windows.Forms.TextBox();
            this.tbSlot = new System.Windows.Forms.TextBox();
            this.tbBottomH = new System.Windows.Forms.TextBox();
            this.tbTopH = new System.Windows.Forms.TextBox();
            this.tbTotalH = new System.Windows.Forms.TextBox();
            this.tbTopLeftW = new System.Windows.Forms.TextBox();
            this.tbTopRightW = new System.Windows.Forms.TextBox();
            this.tbBottomTotalW = new System.Windows.Forms.TextBox();
            this.tbHoleDia = new System.Windows.Forms.TextBox();
            this.btnPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnBuild = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();

            this.SuspendLayout();

            //
            // layout
            //
            this.layout.ColumnCount = 1;
            this.layout.RowCount = 3;
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(
                System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(
                System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(
                System.Windows.Forms.SizeType.Absolute, 50F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(
                System.Windows.Forms.SizeType.Absolute, 22F));
            this.layout.Padding = new System.Windows.Forms.Padding(8);

            //
            // gbParams
            //
            this.gbParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbParams.Text = "Параметры детали (мм)";
            this.gbParams.Controls.Add(this.paramsLayout);

            //
            // paramsLayout
            //
            this.paramsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paramsLayout.ColumnCount = 4;
            this.paramsLayout.RowCount = 5;
            this.paramsLayout.Padding = new System.Windows.Forms.Padding(8, 16, 8, 8);
            for (int i = 0; i < 4; i++)
                this.paramsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(
                    System.Windows.Forms.SizeType.Percent, 25F));
            for (int i = 0; i < 5; i++)
                this.paramsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(
                    System.Windows.Forms.SizeType.Percent, 20F));

            ConfigureLabel(this.lblOuterR, "Внешний R (R20):");
            ConfigureLabel(this.lblInnerR, "Внутренний R (R10):");
            ConfigureLabel(this.lblSlot, "Высота прорези:");
            ConfigureLabel(this.lblBottomH, "Высота нижней части:");
            ConfigureLabel(this.lblTopH, "От центра ⌀ вверх:");
            ConfigureLabel(this.lblTotalH, "Полная высота:");
            ConfigureLabel(this.lblTopLeftW, "Ширина левая верх:");
            ConfigureLabel(this.lblTopRightW, "Ширина правая верх:");
            ConfigureLabel(this.lblBottomTotalW, "Ширина опоры:");
            ConfigureLabel(this.lblHoleDia, "Диаметр отверстия:");

            ConfigureTextBox(this.tbOuterR);
            ConfigureTextBox(this.tbInnerR);
            ConfigureTextBox(this.tbSlot);
            ConfigureTextBox(this.tbBottomH);
            ConfigureTextBox(this.tbTopH);
            ConfigureTextBox(this.tbTotalH);
            ConfigureTextBox(this.tbTopLeftW);
            ConfigureTextBox(this.tbTopRightW);
            ConfigureTextBox(this.tbBottomTotalW);
            ConfigureTextBox(this.tbHoleDia);

            this.paramsLayout.Controls.Add(this.lblOuterR, 0, 0);
            this.paramsLayout.Controls.Add(this.tbOuterR, 1, 0);
            this.paramsLayout.Controls.Add(this.lblInnerR, 2, 0);
            this.paramsLayout.Controls.Add(this.tbInnerR, 3, 0);
            this.paramsLayout.Controls.Add(this.lblSlot, 0, 1);
            this.paramsLayout.Controls.Add(this.tbSlot, 1, 1);
            this.paramsLayout.Controls.Add(this.lblHoleDia, 2, 1);
            this.paramsLayout.Controls.Add(this.tbHoleDia, 3, 1);
            this.paramsLayout.Controls.Add(this.lblBottomH, 0, 2);
            this.paramsLayout.Controls.Add(this.tbBottomH, 1, 2);
            this.paramsLayout.Controls.Add(this.lblTopH, 2, 2);
            this.paramsLayout.Controls.Add(this.tbTopH, 3, 2);
            this.paramsLayout.Controls.Add(this.lblTotalH, 0, 3);
            this.paramsLayout.Controls.Add(this.tbTotalH, 1, 3);
            this.paramsLayout.Controls.Add(this.lblBottomTotalW, 2, 3);
            this.paramsLayout.Controls.Add(this.tbBottomTotalW, 3, 3);
            this.paramsLayout.Controls.Add(this.lblTopLeftW, 0, 4);
            this.paramsLayout.Controls.Add(this.tbTopLeftW, 1, 4);
            this.paramsLayout.Controls.Add(this.lblTopRightW, 2, 4);
            this.paramsLayout.Controls.Add(this.tbTopRightW, 3, 4);

            //
            // btnPanel
            //
            this.btnPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.btnPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);

            this.btnConnect.Text = "Подключиться";
            this.btnConnect.Width = 150;
            this.btnConnect.Height = 32;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            this.btnBuild.Text = "Построить";
            this.btnBuild.Width = 120;
            this.btnBuild.Height = 32;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);

            this.btnClear.Text = "Очистить";
            this.btnClear.Width = 100;
            this.btnClear.Height = 32;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            this.btnSave.Text = "Сохранить как…";
            this.btnSave.Width = 140;
            this.btnSave.Height = 32;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnPanel.Controls.Add(this.btnConnect);
            this.btnPanel.Controls.Add(this.btnBuild);
            this.btnPanel.Controls.Add(this.btnClear);
            this.btnPanel.Controls.Add(this.btnSave);

            //
            // statusStrip
            //
            this.lblStatus.Text = "Готово. Нажмите «Подключиться» для запуска SolidWorks.";
            this.statusStrip.Items.Add(this.lblStatus);
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip.SizingGrip = false;

            this.layout.Controls.Add(this.gbParams, 0, 0);
            this.layout.Controls.Add(this.btnPanel, 0, 1);
            this.layout.Controls.Add(this.statusStrip, 0, 2);

            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 360);
            this.Controls.Add(this.layout);
            this.MinimumSize = new System.Drawing.Size(680, 360);
            this.Text = "Лаб. №4 — 2D-эскиз детали (SolidWorks API)";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private static void ConfigureLabel(System.Windows.Forms.Label label, string text)
        {
            label.Text = text;
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        private static void ConfigureTextBox(System.Windows.Forms.TextBox tb)
        {
            tb.Dock = System.Windows.Forms.DockStyle.Fill;
            tb.Margin = new System.Windows.Forms.Padding(0, 2, 8, 2);
        }
    }
}
