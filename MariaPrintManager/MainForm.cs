using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Management;
using System.Runtime.Serialization;
using Microsoft.Win32;
using System.Net.Http;
using System.Net;

namespace MariaPrintManager
{
    public partial class MainForm : Form
    {
        private PSFile ps = new PSFile();
        PreviewBox previewBox;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            previewBox = new PreviewBox(pictureBoxPreview);
            this.Icon = Properties.Resources.Icon1;
            this.Text = Properties.Resources.Title;
            labelStatus.Text = "";

            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Printer where PrintProcessor = '" + "mariaprint" + "'");
                ManagementObjectCollection moc = mos.Get();
                foreach (ManagementObject mo in moc)
                {
                    comboPrinter.Items.Add(mo["Name"].ToString());
                }
            }
            catch
            {
                //
            }

            comboPaper.SelectedIndex = 0;

            comboColor.Items.Add("カラー");
            comboColor.Items.Add("モノクロ");
            comboColor.SelectedIndex = (int)PSFile.Color.MONO;

            comboDuplex.Items.Add("片面");
            comboDuplex.Items.Add("両面(長辺とじ)");
            comboDuplex.Items.Add("両面(短辺とじ)");
            comboDuplex.SelectedIndex = (int)PSFile.Duplex.SIMPLEX;

            if (System.Environment.GetCommandLineArgs().Count() > 1)
            {
                ps.FileName = System.Environment.GetCommandLineArgs()[1];
            }
#if DEBUG
            else
            {
                ps.FileName = "C:\\debug\\debug.ps";
                comboPrinter.Items.Add("Microsoft Print to PDF");
                comboPrinter.Items.Add("Microsoft XPS Document Writer");
                comboPrinter.Items.Add("Bad Dummy Printer");
            }
#endif
            if (comboPrinter.Items.Count > 0)
            {
                comboPrinter.SelectedIndex = 0;
                comboPrinter.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FILEUTIL.SafeDelete(this.ps.TmpDirName);
            FILEUTIL.SafeDelete(this.ps.InkFileName);
#if DEBUG
#else
            FILEUTIL.SafeDelete(this.ps.FileName);
            FILEUTIL.SafeDelete(this.ps.IniFileName);
#endif
        }

        private bool isUserNameEnabled = false;
        private bool isPasswordEnabled = false;
        private bool isPrintEnabled = false;
        private bool isPrinterEnabled = false;
        private bool isColorEnabled = false;
        private bool isDuplexEnabled = false;
        private void EnableControls(bool enabled)
        {
            if (enabled)
            {
                textUserName.Enabled = isUserNameEnabled;
                textPassword.Enabled = isPasswordEnabled;
                buttonPrint.Enabled = isPrintEnabled;
                comboPrinter.Enabled = isPrinterEnabled;
                comboColor.Enabled = isColorEnabled;
                comboDuplex.Enabled = isDuplexEnabled;
            }
            else
            {
                isUserNameEnabled = textUserName.Enabled;
                isPasswordEnabled = textPassword.Enabled;
                isPrintEnabled = buttonPrint.Enabled;
                isPrinterEnabled = comboPrinter.Enabled;
                isColorEnabled = comboColor.Enabled;
                isDuplexEnabled = comboDuplex.Enabled;

                textUserName.Enabled = false;
                textPassword.Enabled = false;
                buttonPrint.Enabled = false;
                comboPrinter.Enabled = false;
                comboColor.Enabled = false;
                comboDuplex.Enabled = false;
            }
            this.UseWaitCursor = !enabled;
            this.Refresh();
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            this.Activate();
            this.TopMost = true;
            this.Activate();
            this.TopMost = false;

            EnableControls(false);

            comboPaper.SelectedIndex = INIFILE.GetValue("DEVMODE", "PaperSize", 9, this.ps.IniFileName);
            previewBox.Orientation = INIFILE.GetValue("DEVMODE", "Orientation", 1, this.ps.IniFileName);
            string documentname = INIFILE.GetValue("JOBINFO", "Document", "", this.ps.IniFileName);
            if (documentname.Length > 0)
            {
                this.Text = Properties.Resources.Title + " - " + documentname;
            }

            if (this.ps.IsValid())
            {
                this.Refresh();
                labelStatus.Text = "ファイル解析中";
                statusStrip1.Refresh();

                /*
                 * プレビュー作成
                 */
                labelAnalysis.Text = "印刷データを読み込んでいます…";
                timer1.Interval = 3000;
                timer1.Enabled = true;
                this.Refresh();
                try
                {
                    // サムネイル作成
                    System.IO.Directory.CreateDirectory(this.ps.TmpDirName);
                    string output = System.IO.Path.Combine(ps.TmpDirName, "def-%04d.png");

                    bool b = await ps.ExportPreview1(output);
                }
                catch (ExternalException ex)
                {
                    if (ex.HResult != -100)
                    {
                        labelPreviewError.Visible = true;
                    }
                }
                catch
                {
                    labelPreviewError.Visible = true;
                }

                try
                {
                    if (System.IO.Directory.GetFiles(ps.TmpDirName, "*", System.IO.SearchOption.TopDirectoryOnly).Length > 0)
                    {
                        previewBox.FileName = System.IO.Path.Combine(ps.TmpDirName, "def-0001.png");
                    }
                    FILEUTIL.SafeDelete(ps.TmpDirName);
                }
                catch
                {
                    // do nothing
                }
                timer1.Stop();
                timer1.Enabled = false;


                /*
                 * ページ数算出
                 */
                labelAnalysis.Text = "ページ数をカウントしています";
                timer2.Interval = 1000;
                timer2.Enabled = true;
                this.Refresh();
                try
                {
                    bool b = await ps.Analyze();

                    labelAnalysis.Text = "計 " + this.ps.Pages.Total + " 枚";

                    DisplayPageAndCost();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("印刷データの解析ができませんでした\r\n\r\n理由：" + ex.Message + "\r\n詳細：" + ex.HResult.ToString(), Properties.Resources.Title);
                    labelStatus.Text = "解析エラー";
                }
                timer1.Enabled = false;
                timer2.Enabled = false;
                statusStrip1.Refresh();
                labelAnalysis.Text = "";
                this.Refresh();
            }

            this.Activate();
            this.TopMost = true;
            this.Activate();
            this.textUserName.Focus();
            this.Activate();
            this.TopMost = false;

            EnableControls(true);

            if (this.ps.Pages.Total > 0)
            {
                textPassword.Enabled = true;
                textUserName.Enabled = true;
                buttonPrint.Enabled = true;
                textUserName.Focus();
            }
        }

        private async void DisplayPageAndCost()
        {
            if (this.ps.Pages.Total > 0)
            {
                labelPageCount.Text = "計 " + this.ps.Pages.Total.ToString() + " 枚";
                labelStatus.Text =
                    "カラー: " + this.ps.Pages.Color.ToString() + " 枚　" +
                    "モノクロ: " + this.ps.Pages.Mono.ToString() + " 枚　" +
                    "ブランク: " + this.ps.Pages.Blank.ToString() + " 枚　" +
                    "計: " + this.ps.Pages.Total.ToString() + " 枚";

                int cost = -1;
                try
                {
                    int color_count = this.ps.Pages.Color;
                    int mono_count = this.ps.Pages.Mono;
                    if (comboColor.SelectedIndex == (int)PSFile.Color.MONO)
                    {
                        mono_count += color_count;
                        color_count = 0;
                    }
                    if (printerIsValid())
                    {
                        cost = await WebAPI.Quotation(color_count, mono_count, this.ps.Pages.Blank, comboPaper.SelectedIndex, comboPrinter.SelectedItem.ToString());
                    }
                }
                catch
                {
                    // do nothing
                }
                if (cost >= 0)
                {
                    labelStatus.Text += "　" + cost.ToString() + " Points";
                }
            }
            else
            {
                labelStatus.Text = "";
            }
        }

        private bool printerIsValid()
        {
            try
            {
                if (comboPrinter.SelectedIndex < 0)
                {
                    return false;
                }
                System.Drawing.Printing.PrinterSettings pset = new System.Drawing.Printing.PrinterSettings();
                pset.PrinterName = comboPrinter.SelectedItem.ToString();
                return pset.IsValid;
            }
            catch
            {
                return false;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            trimLabel();
        }

        private void labelAnalysis_SizeChanged(object sender, EventArgs e)
        {
            trimLabel();
        }

        private void trimLabel()
        {
            try
            {
                labelAnalysis.Left = (pictureBoxPreview.Width - labelAnalysis.Width) / 2;
                labelAnalysis.Top = (pictureBoxPreview.Height - labelAnalysis.Height) / 2;
            }
            catch
            {
                // do nothing
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelAnalysis.Text += "…しばらくお待ちください";
            timer1.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                string newtext = "";
                if (System.IO.File.Exists(ps.InkFileName))
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(ps.InkFileName);
                    if (System.Text.RegularExpressions.Regex.IsMatch(labelAnalysis.Text, @"\w\s\d+"))
                    {
                        newtext = System.Text.RegularExpressions.Regex.Replace(labelAnalysis.Text, @"(\w)\s\d+", "$1");
                    }
                    else
                    {
                        newtext = labelAnalysis.Text;
                    }
                    labelAnalysis.Text = newtext + " " + (info.Length / 44).ToString();
                }
            }
            catch
            {
                // do nothing
            }
        }

        private bool mouseDown = false;
        private void pictureBoxPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBoxPreview.Image != null)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        pictureBoxPreview.Cursor = Cursors.PanWest;
                        break;
                    case MouseButtons.Right:
                        pictureBoxPreview.Cursor = Cursors.PanEast;
                        break;
                }
                mouseDown = true;
            }
        }

        private void pictureBoxPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBoxPreview.Image != null && mouseDown)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        previewBox.Rotate(RotateFlipType.Rotate270FlipNone);
                        break;
                    case MouseButtons.Right:
                        previewBox.Rotate(RotateFlipType.Rotate90FlipNone);
                        break;
                }
                pictureBoxPreview.Cursor = Cursors.Default;
                pictureBoxPreview.Refresh();
            }
            mouseDown = false;
        }

        private async void buttonPrint_Click(object sender, EventArgs e)
        {
            bool duplexSelectable = comboDuplex.Enabled;
            bool colorSelectable = comboColor.Enabled;

            EnableControls(false);
            string statusText = labelStatus.Text;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var pwd = new char[32];
            var random = new Random();
            for (int i = 0; i < pwd.Length; i++)
            {
                pwd[i] = chars[random.Next(chars.Length)];
            }
            string jobname = new string(pwd);

            try
            {
                string printer = null;

                if (comboPrinter.SelectedIndex >= 0)
                {
                    printer = comboPrinter.SelectedItem.ToString();
                }
                else
                {
                    throw new Exception("プリンタが選択されていません");
                }

                System.Drawing.Printing.PrinterSettings pset = new System.Drawing.Printing.PrinterSettings();
                pset.PrinterName = printer;
                if (!pset.IsValid)
                {
                    throw new Exception("選択されたプリンタは有効なプリンタではありません");
                }
                bool supported_paper = false;
                for (int i=0; i<pset.PaperSizes.Count; i++)
                {
                    if (pset.PaperSizes[i].RawKind == comboPaper.SelectedIndex)
                    {
                        supported_paper = true;
                        break;
                    }
                }
                if (!supported_paper)
                {
                    throw new Exception("選択されたプリンタはこの用紙サイズを印刷できません\n\n" +
                        "用紙サイズ: " + comboPaper.SelectedItem.ToString());
                }

                int color_count = this.ps.Pages.Color;
                int mono_count = this.ps.Pages.Mono;
                if (comboColor.SelectedIndex == (int)PSFile.Color.MONO)
                {
                    mono_count += color_count;
                    color_count = 0;
                }
                WebAPI.PrintInfo info = await WebAPI.Payment(textUserName.Text, textPassword.Text, color_count, mono_count, this.ps.Pages.Blank, comboPaper.SelectedIndex, comboPrinter.SelectedItem.ToString(), INIFILE.GetValue("JOBINFO", "Document", "", this.ps.IniFileName));
                if (info == null) 
                {
                    throw new Exception("認証サーバに接続できませんでした。(コード：" + WebAPI.StatusCode.ToString() + ")");
                }
                if (info.result != "OK")
                {
                    throw new Exception(info.message.Replace("\\n", "\r\n"));
                }
                labelAnalysis.Text = info.message.Replace("\\n", "\r\n");

                // Task t = Task.Run(() => MessageBox.Show(labelAnalysis.Text));

                REG.PrinterName = printer;
                REG.JobName = jobname;

                labelStatus.Text = "印刷中...";
                statusStrip1.Refresh();
                try
                {
                    this.ps.Print(printer, jobname,
                        comboPaper.SelectedIndex < 0 ? 9 : comboPaper.SelectedIndex,
                        comboColor.SelectedIndex < 0 ? PSFile.Color.AUTO : (PSFile.Color)comboColor.SelectedIndex,
                        comboDuplex.SelectedIndex < 0 ? PSFile.Duplex.SIMPLEX : (PSFile.Duplex)comboDuplex.SelectedIndex);
#if DEBUG
#else
                    FILEUTIL.SafeDelete(this.ps.FileName);
                    FILEUTIL.SafeDelete(this.ps.IniFileName);
#endif
                    labelAnalysis.Text = "印刷が送信されました";
                    await Task.Delay(2000);
                    this.Close();
                }
                catch (Exception ex)
                {
                    labelStatus.Text = "印刷時エラー";
                    statusStrip1.Refresh();
                    throw new Exception("印刷処理ができませんでした\r\n\r\n理由：" + ex.Message + "\r\n詳細：" + ex.HResult.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                string printer = "";
                if (comboPrinter.SelectedIndex >= 0) {
                    printer = comboPrinter.SelectedItem.ToString();
                }
                try
                {
                    bool b = await WebAPI.Alert(textUserName.Text, this.ps.Pages.Color, this.ps.Pages.Mono, this.ps.Pages.Blank, comboPaper.SelectedIndex, printer, INIFILE.GetValue("JOBINFO", "Document", "", this.ps.IniFileName), ex.Message);
                }
                catch
                {
                    //
                }
            }
            finally
            {
                EnableControls(true);
                labelStatus.Text = statusText;
            }
        }

        private void comboPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (printerIsValid())
            {
                System.Drawing.Printing.PrinterSettings pset = new System.Drawing.Printing.PrinterSettings();
                pset.PrinterName = comboPrinter.SelectedItem.ToString();
                if (pset.CanDuplex)
                {
                    comboDuplex.Enabled = true;
                }
                else
                {
                    if (comboDuplex.Items.Count > 0)
                    {
                        comboDuplex.SelectedIndex = (int)PSFile.Duplex.SIMPLEX;
                    }
                    comboDuplex.Enabled = false;

                }
                if (pset.SupportsColor)
                {
                    comboColor.Enabled = true;
                }
                else
                {
                    comboColor.Enabled = false;
                }
            }
            else
            {
                comboDuplex.Enabled = false;
                comboColor.Enabled = false;
            }
            DisplayPageAndCost();
        }

        private void comboColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayPageAndCost();
        }

        private void comboDuplex_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayPageAndCost();
        }

        private void comboPaper_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayPageAndCost();
        }

        private void textUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textPassword.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void textPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                buttonPrint.Select();
                e.SuppressKeyPress = true;
            }
        }

        private void labelAnalysis_TextChanged(object sender, EventArgs e)
        {
            if (((Label)sender).Text.Length > 0)
            {
                ((Label)sender).Visible = true;
            }
            else
            {
                ((Label)sender).Visible = false;
            }
            ((Label)sender).Refresh();
        }
    }



    /*
     * class PSFile
     */
    class PSFile
    {
        private string fileName = null;
        private PagesStore pages = new PagesStore();

        public enum Duplex : int { SIMPLEX = 0, VERTICAL, HORIZONTAL };
        public enum Color : int { AUTO = 0, MONO };

        public class PagesStore
        {
            public int Mono = 0;
            public int Color = 0;
            public int Blank = 0;
            public int Total
            {
                get { return Mono + Color + Blank; }
            }
        }

        public string FileName
        {
            set { this.fileName = value; }
            get { return this.fileName; }
        }

        public string TmpDirName
        {
            get { return FileName + ".extract"; }
        }

        public string InkFileName
        {
            get { return FileName + ".ink"; }
        }

        public string IniFileName
        {
            get {
                try
                {
                    return System.IO.Path.Combine(System.IO.Path.GetDirectoryName("" + FileName), System.IO.Path.GetFileNameWithoutExtension("" + FileName)) + ".ini";
                }
                catch
                {
                    return null;
                }
            }
        }

        public PagesStore Pages
        {
            get { return pages; }
        }

        public bool IsValid()
        {
            return System.IO.File.Exists(fileName);
        }

        public async Task<bool> ExportPreview1(string output)
        {
            await Task.Run(() =>
            {
                string[] args = {
                    "gs",
                    "-dBATCH",
                    "-dSAFER",
                    "-sDEVICE=png16m",
                    "-sOutputFile=" + output,
                    "-dTextAlphaBits=4",
                    "-dGraphicsAlphaBits=4",
                    "-r120",
                    "-dFirstPage=1",
                    "-dLastPage=1",
                    "-f",
                    FileName
                };
                int result = GSDLL.Execute(args);
            });

            return true;
        }

        public async Task<bool> Analyze()
        {
            bool ret = false;

            await Task.Run(() =>
            {
                // インク消費量からカラー/モノクロのページ数を算出
                string[] args = {
                    "gs",
                    "-dBATCH",
                    "-dNOPAUSE",
                    "-dSAFER",
                    "-dNoCancel",
                    "-sDEVICE=inkcov",
                    "-sOutputFile=" + InkFileName,
                    "-r75",
                    "-f",
                    FileName
                };
                int result = GSDLL.Execute(args);

                Microsoft.VisualBasic.FileIO.TextFieldParser tfp = new Microsoft.VisualBasic.FileIO.TextFieldParser(InkFileName);
                tfp.Delimiters = new string[] { " " };
                while (!tfp.EndOfData)
                {
                    string[] fields = tfp.ReadFields();
                    double[] ink = { 0.0, 0.0, 0.0, 0.0 };
                    int i = 0;
                    foreach (string s in fields)
                    {
                        if (s.Length > 0)
                        {
                            ink[i] = Convert.ToDouble(s);
                            i++;
                        }
                        if (i > 3)
                        {
                            break;
                        }
                    }
                    if (ink[0] + ink[1] + ink[2] > 0.0)
                    {
                        Pages.Color++;
                    }
                    else if (ink[3] > 0.0)
                    {
                        Pages.Mono++;
                    }
                    else
                    {
                        Pages.Blank++;
                    }
                }
                tfp.Close();
                ret = true;
                FILEUTIL.SafeDelete(InkFileName);
            });

            return ret;
        }

        public void Print(string printer, string jobname, int paper = 0, Color color = Color.AUTO, Duplex duplex = Duplex.SIMPLEX)
        {
            if (System.IO.File.Exists(fileName))
            {

                string duplex1 = "";
                switch(duplex)
                {
                    case Duplex.SIMPLEX:
                        break;
                    case Duplex.VERTICAL:
                        duplex1 = "/Duplex true /Tumble false ";
                        break;
                    case Duplex.HORIZONTAL:
                        duplex1 = "/Duplex true /Tumble true ";
                        break;
                }

                string color1 = "/BitsPerPixel 24 ";
                string color2 = "/Color 2 ";
                if (color == Color.MONO)
                {
                    color2 = "/Color 1 ";
                }

                string paper1 = "/Paper 9 ";
                if (paper > 0)
                {
                    paper1 = "/Paper " + Convert.ToInt32(paper) + " ";
                }

                string[] args = {
                    "gs",
                    "-c",
                    "mark " +
                        "/NoCancel true " +
                        "/OutputFile (%printer%" + (printer == null ? "" : printer) + ") " +
                        color1 +
                        duplex1 +
                        "/UserSettings " +
                        "<<" +
                            "/DocumentName (" + jobname + ") " +
                            paper1 +
                            "/Orientation 1 " +
                            color2 +
                            duplex1 +
                        ">> " +
                        "(mswinpr2) finddevice " +
                        "putdeviceprops " +
                        "setdevice",
                    "-dBATCH",
                    "-dNOPAUSE",
                    "-dSAFER",
                    "-f",
                    this.fileName
                };
                GSDLL.Execute(args);
            }
        }
    }



    /*
     * class PreviewBox
     */
    class PreviewBox
    {
        private PictureBox box = null;
        private Image image = null;
        private Timer timerRotate = new Timer();
        private string fileName = null;
        private int orientation = 1;

        public PreviewBox(PictureBox box)
        {
            this.box = box;
            Orientation = 1;
        }

        public int Orientation {
            set
            {
                switch (value)
                {
                    case 1:
                    case 2:
                        this.orientation = value;
                        break;
                }

                if (RotateByOrientation())
                {
                    box.Image = this.image;
                }
            }
            get
            {
                return this.orientation;
            }
        }

        private bool RotateByOrientation()
        {
            bool isrotate = false;

            if (this.image != null)
            {
                switch (orientation)
                {
                    case 1:
                        if (this.image.Height < this.image.Width)
                        {
                            this.image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            isrotate = true;
                        }
                        break;
                    case 2:
                        if (this.image.Height > this.image.Width)
                        {
                            this.image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            isrotate = true;
                        }
                        break;
                }
            }

            return isrotate;
        }

        public string FileName
        {
            set
            {
                try
                {
                    box.Image = null;

                    System.IO.FileStream fs = new System.IO.FileStream(
                        value,
                        System.IO.FileMode.Open,
                        System.IO.FileAccess.Read);
                    this.image = System.Drawing.Image.FromStream(fs);
                    fs.Close();

                    RotateByOrientation();
                    box.Image = this.image;

                    this.fileName = value;

                    box.Refresh();
                }
                catch
                {
                    this.fileName = null;
                    this.image = null;
                }
            }
            get
            {
                return fileName;
            }
        }

        public void Rotate(RotateFlipType type)
        {
            box.Image.RotateFlip(type);
        }
    }



    /*
     * class WebAPI
     */
    class WebAPI
    {
        /*
         * get printing cost
         * POST {baseurl}/quotation
         * 
         * print
         * POST {baseurl}/payment
         * 
         * post alert
         * POST {baseurl}/alert
         */

        private static HttpClient client = new HttpClient(new HttpClientHandler() { UseCookies = false });
        public static int StatusCode { protected set; get; } = 0;

        [DataContract]
        internal class PrintInfo
        {
            [DataMember] public string cost { get; set; }
            [DataMember] public string points { get; set; }
            [DataMember] public string result { get; set; }
            [DataMember] public string message { get; set; }
        }

        public static async Task<int> Quotation(int color_count, int mono_count, int blank_count, int paper, string printer)
        {
            PrintInfo info = null;

            for (int i=0; i<3; i++)
            {
                info = await ExecutePost("quotation", "", "", color_count, mono_count, blank_count, paper, printer, "", "");
                if (StatusCode != -53)
                {
                    break;
                }
            }

            int cost = -1;
            if (info != null)
            {
                if (!Int32.TryParse(info.cost, out cost))
                {
                    cost = -1;
                }
            }

            return cost;
        }

        public static async Task<PrintInfo> Payment(string username, string password, int color_count, int mono_count, int blank_count, int paper, string printer, string document)
        {
            PrintInfo info = null;

            for (int i=0; i<3; i++)
            {
                info = await ExecutePost("payment", username, password, color_count, mono_count, blank_count, paper, printer, document, "");
                if (StatusCode != -53)
                {
                    break;
                }
            }

            return info;
        }

        public static async Task<bool> Alert(string username, int color_count, int mono_count, int blank_count, int paper, string printer, string document, string alert)
        {
            PrintInfo info = null;

            for (int i=0; i<3; i++)
            {
                info = await ExecutePost("alert", username, "", color_count, mono_count, blank_count, paper, printer, document, alert);
                if (StatusCode != -53)
                {
                    break;
                }
            }

            if (info == null)
            {
                return false;
            }

            return true;
        }

        private static async Task<PrintInfo> ExecutePost(string path, string username, string password, int color_count, int mono_count, int blank_count, int paper, string printer, string document, string alert)
        {
            PrintInfo info = null;

            StatusCode = 0;

            try
            {
                Uri baseUri = new Uri(new Uri(REG.BaseURL), path);
                ServicePoint sp = ServicePointManager.FindServicePoint(baseUri);
                sp.ConnectionLeaseTimeout = 60 * 1000;
#if DEBUG
                Console.WriteLine(baseUri.ToString());
#endif

                Dictionary<string, string> param = new Dictionary<string, string>()
                {
                    { "username", username },
                    { "password", password },
                    { "hostname", System.Net.Dns.GetHostName() },
                    { "color_count", color_count.ToString() },
                    { "mono_count", mono_count.ToString() },
                    { "blank_count", blank_count.ToString() },
                    { "paper", paper.ToString() },
                    { "roomid", REG.RoomID },
                    { "printer", printer },
                    { "driver", GetPrinterDriver(printer) },
                    { "document", document },
                    { "alert", alert },
                };
                FormUrlEncodedContent content = new FormUrlEncodedContent(param);

                HttpResponseMessage response = null;
                try
                {
                    response = await client.PostAsync(baseUri, content);
                }
                catch (HttpRequestException)
                {
#if DEBUG
                    Console.WriteLine("PostAsync error");
#endif
                    StatusCode = -53;
                    throw;
                }

                StatusCode = (int)response.StatusCode;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync();
#if DEBUG
                    Console.WriteLine(json);
#endif
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(PrintInfo));
                    using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        info = (PrintInfo)serializer.ReadObject(ms);
                        info.message.Replace("\\n", "\r\n");
                    }
                }
            }
            catch
            {
                // do nothing
            }

            return info;
        }

        private static string GetPrinterDriver(string printer)
        {
            string driver = null;
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Printer where Name='" + printer + "'");
                ManagementObjectCollection moc = mos.Get();
                if (moc.Count <= 0)
                {
                    driver = null;
                }
                else
                {
                    foreach (ManagementObject mo in moc)
                    {
                        driver = mo["DriverName"].ToString();
                        break;
                    }
                }
            }
            catch
            {
                //
            }
            return driver;
        }
    }



    /*
     * class REG
     */
    class REG
    {
        public static string JobName
        {
            set
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    RegistryKey vendorKey = baseKey.CreateSubKey(@"SOFTWARE\" + Properties.Resources.Vendor, true, RegistryOptions.Volatile);
                    RegistryKey productKey = vendorKey.CreateSubKey(Properties.Resources.Product, true, RegistryOptions.Volatile);

                    string[] names = (string[])productKey.GetValue(Properties.Resources.RegValueDocumentNames, new string[] { });

                    bool hit = false;
                    foreach (string s in names)
                    {
                        if (s == value)
                        {
                            hit = true;
                            break;
                        }
                    }
                    if (!hit)
                    {
                        Array.Resize(ref names, names.Length + 1);
                        names[names.Length - 1] = value;
                    }

                    productKey.SetValue(Properties.Resources.RegValueDocumentNames, names, RegistryValueKind.MultiString);

                    productKey.Close();
                    vendorKey.Close();
                }
            }
        }

        public static string PrinterName
        {
            set
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    RegistryKey vendorKey = baseKey.CreateSubKey(@"SOFTWARE\" + Properties.Resources.Vendor, true, RegistryOptions.Volatile);
                    RegistryKey productKey = vendorKey.CreateSubKey(Properties.Resources.Product, true, RegistryOptions.Volatile);

                    productKey.SetValue(Properties.Resources.RegValuePrinter, value);

                    productKey.Close();
                    vendorKey.Close();
                }
            }
        }

        public static string BaseURL
        {
            get
            {
                return (string)Microsoft.Win32.Registry.GetValue(Properties.Resources.RegKeyMariaPrintSystem, Properties.Resources.RegValueBaseUrl, null);
            }
        }

        public static string RoomID
        {
            get
            {
                return (string)Microsoft.Win32.Registry.GetValue(Properties.Resources.RegKeyMariaPrintSystem, Properties.Resources.RegValueRoomID, null);
            }
        }
    }



    /*
     * class INIFILE
     */
    class INIFILE
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        public static string GetValue(string section, string key, string default_value, string inifile)
        {
            if (!System.IO.File.Exists(inifile))
            {
                return default_value;
            }

            StringBuilder sb = new StringBuilder(1024);

            GetPrivateProfileString(section, key, default_value, sb, Convert.ToUInt32(sb.Capacity), inifile);

            return sb.ToString();
        }

        public static int GetValue(string section, string key, int default_value, string inifile)
        {
            int value;

            if (!int.TryParse(GetValue(section, key, default_value.ToString(), inifile), out value))
            {
                return default_value;
            }

            return value;
        }
    }



    /*
     * class FILEUTIL
     */
    class FILEUTIL
    {
        public static bool SafeDelete(string filename, bool recursive = true)
        {
            if (System.IO.Directory.Exists(filename))
            {
                try
                {
                    System.IO.Directory.Delete(filename, recursive);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else if (System.IO.File.Exists(filename))
            {
                try
                {
                    System.IO.File.Delete(filename);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }

    /*
     * class GSDLL
     */
    class GSDLL
    {
        class GSDLL32
        {
            [DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
            public static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

            [DllImport("gsdll64.dll", EntryPoint = "gsapi_set_arg_encoding")]
            private static extern int gsapi_set_arg_encoding(IntPtr inst, int encoding);

            [DllImport("gsdll32.dll", EntryPoint = "gsapi_init_with_args")]
            public static extern int gsapi_init_with_args(IntPtr instance, int argc, string[] argv);

            [DllImport("gsdll32.dll", EntryPoint = "gsapi_exit")]
            public static extern int gsapi_exit(IntPtr instance);

            [DllImport("gsdll32.dll", EntryPoint = "gsapi_delete_instance")]
            public static extern void gsapi_delete_instance(IntPtr instance);

            public static int Execute(string[] argv)
            {
                IntPtr gs;
                int result;

                lock (gsinstance)
                {
                    try
                    {
                        gsapi_new_instance(out gs, IntPtr.Zero);
                        gsapi_set_arg_encoding(gs, GS_ARG_ENCODING_UTF8);
                        result = gsapi_init_with_args(gs, argv.Length, argv);
                        gsapi_exit(gs);
                        gsapi_delete_instance(gs);
                    }
                    catch
                    {
                        result = -255;
                    }
                }

                return result;
            }
            private static object gsinstance = new object();
            private const int GS_ARG_ENCODING_UTF8 = 1;
        }

        class GSDLL64
        {
            [DllImport("gsdll64.dll", EntryPoint = "gsapi_new_instance")]
            public static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

            [DllImport("gsdll64.dll", EntryPoint = "gsapi_set_arg_encoding")]
            private static extern int gsapi_set_arg_encoding(IntPtr inst, int encoding);

            [DllImport("gsdll64.dll", EntryPoint = "gsapi_init_with_args")]
            public static extern int gsapi_init_with_args(IntPtr instance, int argc, string[] argv);

            [DllImport("gsdll64.dll", EntryPoint = "gsapi_exit")]
            public static extern int gsapi_exit(IntPtr instance);

            [DllImport("gsdll64.dll", EntryPoint = "gsapi_delete_instance")]
            public static extern void gsapi_delete_instance(IntPtr instance);

            private static void check(int result)
            {
                if (result != 0)
                {
                    throw new ExternalException("DLL error", result);
                }
            }

            public static int Execute(string[] argv)
            {
                IntPtr gs = IntPtr.Zero;
                int result;

                lock (gsinstance)
                {
                    try
                    {
                        check(gsapi_new_instance(out gs, IntPtr.Zero));
                        check(gsapi_set_arg_encoding(gs, GS_ARG_ENCODING_UTF8));
                        result = gsapi_init_with_args(gs, argv.Length, argv);
                        check(gsapi_exit(gs));
                    }
                    catch (Exception ex)
                    {
                        result = ex.HResult;
                    }
                    finally
                    {
                        gsapi_delete_instance(gs);
                    }
                }

                return result;
            }
            private static object gsinstance = new object();
            private const int GS_ARG_ENCODING_UTF8 = 1;
        }

        public static int Execute(string[] argv)
        {
            int result;

            if (IntPtr.Size == 4)
            {
                result = GSDLL32.Execute(argv);
            }
            else
            {
                result = GSDLL64.Execute(argv);
            }
            if (result == -255)
            {
                throw new ExternalException("DLL error", result);
            }
            else if (result < 0)
            {
                throw new ExternalException("PostScript processing error", result);
            }

            return result;
        }
    }
}
