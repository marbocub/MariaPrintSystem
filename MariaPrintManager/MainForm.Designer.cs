namespace MariaPrintManager
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.comboDuplex = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboColor = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboPaper = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboPrinter = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.labelPageCount = new System.Windows.Forms.Label();
            this.labelAnalysis = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.labelPreviewError = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(584, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(64, 17);
            this.labelStatus.Text = "labelStatus";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.buttonPrint);
            this.panel1.Controls.Add(this.comboDuplex);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.comboColor);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.comboPaper);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comboPrinter);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textPassword);
            this.panel1.Controls.Add(this.textUserName);
            this.panel1.Location = new System.Drawing.Point(0, 434);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 102);
            this.panel1.TabIndex = 2;
            // 
            // buttonPrint
            // 
            this.buttonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrint.Enabled = false;
            this.buttonPrint.Location = new System.Drawing.Point(448, 3);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(124, 96);
            this.buttonPrint.TabIndex = 4;
            this.buttonPrint.Text = "認証して印刷";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // comboDuplex
            // 
            this.comboDuplex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDuplex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDuplex.Enabled = false;
            this.comboDuplex.FormattingEnabled = true;
            this.comboDuplex.Location = new System.Drawing.Point(342, 79);
            this.comboDuplex.Name = "comboDuplex";
            this.comboDuplex.Size = new System.Drawing.Size(100, 20);
            this.comboDuplex.TabIndex = 12;
            this.comboDuplex.SelectedIndexChanged += new System.EventHandler(this.comboDuplex_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(295, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "印刷面";
            // 
            // comboColor
            // 
            this.comboColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboColor.Enabled = false;
            this.comboColor.FormattingEnabled = true;
            this.comboColor.Location = new System.Drawing.Point(199, 79);
            this.comboColor.Name = "comboColor";
            this.comboColor.Size = new System.Drawing.Size(80, 20);
            this.comboColor.TabIndex = 10;
            this.comboColor.SelectedIndexChanged += new System.EventHandler(this.comboColor_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(161, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "カラー";
            // 
            // comboPaper
            // 
            this.comboPaper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPaper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPaper.Enabled = false;
            this.comboPaper.FormattingEnabled = true;
            this.comboPaper.Items.AddRange(new object[] {
            "自動",
            "US Letter",
            "US Letter Small",
            "US Tabloid",
            "US Ledger",
            "US Legal",
            "US Statement",
            "US Executive",
            "A3",
            "A4",
            "A4 Small",
            "A5",
            "B4 (JIS)",
            "B5 (JIS)",
            "Folio",
            "Quarto",
            "10 x 14 in",
            "11 x 17 in",
            "US Note 8",
            "US Envelope #9",
            "US Envelope #10",
            "US Envelope #11",
            "US Envelope #12",
            "US Envelope #14",
            "C size sheet",
            "D size sheet",
            "E size sheet",
            "Envelope DL",
            "Envelope C5",
            "Envelope C3",
            "Envelope C4",
            "Envelope C6",
            "Envelope C65",
            "Envelope B4",
            "Envelope B5",
            "Envelope B6",
            "Envelope 110 x 230 mm",
            "US Envelope Monarch",
            "6 3/4 US Envelope",
            "US Std Fanfold",
            "German Std Fanfold",
            "German Legal Fanfold",
            "B4 (ISO)",
            "Japanese Postcard",
            "9 x 11 in",
            "10 x 11 in",
            "15 x 11 in",
            "Envelope Invite",
            "RESERVED",
            "RESERVED",
            "US Letter Extra",
            "US Legal Extra",
            "US Tabloid Extra",
            "A4 Extra",
            "Letter Transverse",
            "A4 Transverse",
            "Letter Extra Transverse",
            "SuperA/SuperA/A4",
            "SuperB/SuperB/A3",
            "US Letter Plus",
            "A4 Plus",
            "A5 Transverse",
            "B5 (JIS) Transverse",
            "A3 Extra",
            "A5 Extra",
            "B5 (ISO) Extra",
            "A2",
            "A3 Transverse",
            "A3 Extra Transverse",
            "JP Double Postcard",
            "A6",
            "JP Envelope Kaku #2",
            "JP Envelope Kaku #3",
            "JP Envelope Chou #3",
            "JP Envelope Chou #4",
            "Letter Rotated",
            "A3 Rotated",
            "A4 Rotated",
            "A5 Rotated",
            "B4 (JIS) Rotated",
            "B5 (JIS) Rotated",
            "JP Postcard Rotated",
            "Double JP Postcard Rotated",
            "A6 Rotated",
            "JP Envelope Kaku #2 Rotated",
            "JP Envelope Kaku #3 Rotated",
            "JP Envelope Chou #3 Rotated",
            "JP Envelope Chou #4 Rotated",
            "B6 (JIS)",
            "B6 (JIS) Rotated",
            "12 x 11 in",
            "JP Envelope You #4",
            "JP Envelope You #4 Rotated",
            "PRC 16K",
            "PRC 32K",
            "PRC 32K(Big)",
            "PRC Envelope #1",
            "PRC Envelope #2",
            "PRC Envelope #3",
            "PRC Envelope #4",
            "PRC Envelope #5",
            "PRC Envelope #6",
            "PRC Envelope #7",
            "PRC Envelope #8",
            "PRC Envelope #9",
            "PRC Envelope #10",
            "PRC 16K Rotated",
            "PRC 32K Rotated",
            "PRC 32K(Big) Rotated",
            "PRC Envelope #1 Rotated",
            "PRC Envelope #2 Rotated",
            "PRC Envelope #3 Rotated",
            "PRC Envelope #4 Rotated",
            "PRC Envelope #5 Rotated",
            "PRC Envelope #6 Rotated",
            "PRC Envelope #7 Rotated",
            "PRC Envelope #8 Rotated",
            "PRC Envelope #9 Rotated",
            "PRC Envelope #10 Rotated"});
            this.comboPaper.Location = new System.Drawing.Point(87, 79);
            this.comboPaper.Name = "comboPaper";
            this.comboPaper.Size = new System.Drawing.Size(58, 20);
            this.comboPaper.TabIndex = 8;
            this.comboPaper.SelectedIndexChanged += new System.EventHandler(this.comboPaper_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "用紙サイズ";
            // 
            // comboPrinter
            // 
            this.comboPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPrinter.Enabled = false;
            this.comboPrinter.FormattingEnabled = true;
            this.comboPrinter.Location = new System.Drawing.Point(87, 53);
            this.comboPrinter.Name = "comboPrinter";
            this.comboPrinter.Size = new System.Drawing.Size(355, 20);
            this.comboPrinter.TabIndex = 6;
            this.comboPrinter.SelectedIndexChanged += new System.EventHandler(this.comboPrinter_SelectedIndexChanged);
            this.comboPrinter.EnabledChanged += new System.EventHandler(this.comboPrinter_EnabledChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "プリンタ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "パスワード";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "アカウント名";
            // 
            // textPassword
            // 
            this.textPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPassword.Enabled = false;
            this.textPassword.Location = new System.Drawing.Point(87, 28);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(355, 19);
            this.textPassword.TabIndex = 1;
            this.textPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textPassword_KeyDown);
            // 
            // textUserName
            // 
            this.textUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textUserName.Enabled = false;
            this.textUserName.Location = new System.Drawing.Point(87, 3);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(355, 19);
            this.textUserName.TabIndex = 0;
            this.textUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textUserName_KeyDown);
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPreview.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBoxPreview.Location = new System.Drawing.Point(0, 27);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(584, 401);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 3;
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPreview_MouseDown);
            this.pictureBoxPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxPreview_MouseUp);
            // 
            // labelPageCount
            // 
            this.labelPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPageCount.AutoSize = true;
            this.labelPageCount.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelPageCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.labelPageCount.Location = new System.Drawing.Point(515, 400);
            this.labelPageCount.Name = "labelPageCount";
            this.labelPageCount.Padding = new System.Windows.Forms.Padding(5);
            this.labelPageCount.Size = new System.Drawing.Size(57, 22);
            this.labelPageCount.TabIndex = 4;
            this.labelPageCount.Text = "  (0 / 0)";
            this.labelPageCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelAnalysis
            // 
            this.labelAnalysis.AutoSize = true;
            this.labelAnalysis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelAnalysis.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelAnalysis.Location = new System.Drawing.Point(12, 74);
            this.labelAnalysis.Name = "labelAnalysis";
            this.labelAnalysis.Padding = new System.Windows.Forms.Padding(10);
            this.labelAnalysis.Size = new System.Drawing.Size(75, 34);
            this.labelAnalysis.TabIndex = 5;
            this.labelAnalysis.Text = "起動中…";
            this.labelAnalysis.Visible = false;
            this.labelAnalysis.SizeChanged += new System.EventHandler(this.labelAnalysis_SizeChanged);
            this.labelAnalysis.TextChanged += new System.EventHandler(this.labelAnalysis_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // labelPreviewError
            // 
            this.labelPreviewError.AutoSize = true;
            this.labelPreviewError.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelPreviewError.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelPreviewError.Location = new System.Drawing.Point(12, 40);
            this.labelPreviewError.Name = "labelPreviewError";
            this.labelPreviewError.Padding = new System.Windows.Forms.Padding(10);
            this.labelPreviewError.Size = new System.Drawing.Size(147, 34);
            this.labelPreviewError.TabIndex = 6;
            this.labelPreviewError.Text = "プレビューできませんでした";
            this.labelPreviewError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPreviewError.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.labelAnalysis);
            this.Controls.Add(this.labelPreviewError);
            this.Controls.Add(this.labelPageCount);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(600, 480);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.TextBox textUserName;
        private System.Windows.Forms.Label labelPageCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.ComboBox comboPrinter;
        private System.Windows.Forms.Label labelAnalysis;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label labelPreviewError;
        private System.Windows.Forms.ComboBox comboColor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboPaper;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboDuplex;
        private System.Windows.Forms.Label label6;
    }
}