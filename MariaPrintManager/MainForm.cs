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

namespace MariaPrintManager
{
    public partial class MainForm : Form
    {
        private const int AUTO = 0;
        private const int MONO = 1;
        private const int SINGLE_SIDE = 0;
        private const int DOUBLE_SIDE = 1;
        string psfile = null;
        string tmpdir = null;
        string inkfile = null;
        string inifile = null;
        int pages = 0;
        int mono_pages = 0;
        int color_pages = 0;
        int blank_pages = 0;
        int color_unit = 10;
        int mono_unit = 10;
        int blank_unit = 10;
        int total_price = 0;
        bool testFile = false;
        int paperWidth = 2100;
        int paperHeight = 2970;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Icon1;
            this.Text = Properties.Resources.Title;
            toolStripStatusLabel1.Text = "";

            if (System.Environment.GetCommandLineArgs().Count() > 1)
            {
                psfile = System.Environment.GetCommandLineArgs()[1];
            }
            else
            {
                psfile = "C:\\debug\\debug.ps";
                testFile = true;
            }
            tmpdir = psfile + ".extract";
            inkfile = psfile + ".ink";
            inifile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(psfile), System.IO.Path.GetFileNameWithoutExtension(psfile)) + ".ini";

            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Printer where PrintProcessor = '" + "mariaprint" + "'");
                ManagementObjectCollection moc = mos.Get();
                foreach (ManagementObject mo in moc)
                {
                    comboPrinter.Items.Add(mo["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                //
            }
            if (testFile && System.IO.File.Exists(psfile))
            {
                comboPrinter.Items.Add("Microsoft Print to PDF");
                comboPrinter.Items.Add("Microsoft XPS Document Writer");
            }
            if (comboPrinter.Items.Count > 0)
            {
                comboPrinter.SelectedIndex = 0;
                comboPrinter.Enabled = true;
            }

            if (comboPaper.Items.Count > 0)
            {
                comboPaper.SelectedIndex = 0;
            }
            comboColor.Items.Add("自動");
            comboColor.Items.Add("白黒(ディザ)");
            comboColor.SelectedIndex = AUTO;
            comboDuplex.Items.Add("片面");
            comboDuplex.Items.Add("両面");
            comboDuplex.SelectedIndex = SINGLE_SIDE;
        }


        private static bool isUserNameEnabled = false;
        private static bool isPasswordEnabled = false;
        private static bool isPrintEnabled = false;
        private static bool isPrinterEnabled = false;
        private static bool isColorEnabled = false;
        private static bool isDuplexEnabled = false;
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

            if (inifile != null && System.IO.File.Exists(inifile))
            {
                int paper, width, height;

                Int32.TryParse(INIFILE.GetValue("DEVMODE", "PaperSize", "9", inifile), out paper);
                Int32.TryParse(INIFILE.GetValue("DEVMODE", "PaperWidth", "2100", inifile), out width);
                Int32.TryParse(INIFILE.GetValue("DEVMODE", "PaperLength", "2970", inifile), out height);

                comboPaper.SelectedIndex = paper;
                paperWidth = width;
                paperHeight = height;
            }


            if (psfile != null && System.IO.File.Exists(psfile))
            {
                this.Refresh();
                toolStripStatusLabel1.Text = "ファイル解析中";
                statusStrip1.Refresh();

                /*
                 * プレビュー作成
                 */
                labelAnalysis.Text = "印刷データを読み込んでいます…";
                labelAnalysis.Visible = true;
                labelAnalysis.Refresh();
                timer1.Interval = 3000;
                timer1.Enabled = true;
                this.Refresh();
                try
                {
                    // サムネイル作成
                    System.IO.Directory.CreateDirectory(tmpdir);
                    string output = System.IO.Path.Combine(tmpdir, "def-%04d.png");

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
                            "-r96",
                            "-dFirstPage=1",
                            "-dLastPage=1",
                            "-f",
                            psfile
                        };
                        /*
                            "-dUseMediaBox",
                            "-sPAPERSIZE=a4",
                         */
                        int result = GSDLL.Execute(args);
                    });
                }
                catch (ExternalException ex)
                {
                    if (ex.HResult != -100)
                    {
                        labelPreviewError.Visible = true;

                    }
                }
                catch (Exception ex)
                {
                    labelPreviewError.Visible = true;
                }

                try
                {
                    if (System.IO.Directory.GetFiles(tmpdir, "*", System.IO.SearchOption.TopDirectoryOnly).Length > 0)
                    {
                        drawImageFromFile(System.IO.Path.Combine(tmpdir, "def-0001.png"));
                    }
                    System.IO.Directory.Delete(tmpdir, true);
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
                labelAnalysis.Visible = true;
                labelAnalysis.Refresh();
                timer2.Interval = 1000;
                timer2.Enabled = true;
                this.Refresh();
                try
                {
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
                            "-sOutputFile=" + inkfile,
                            "-r75",
                            "-f",
                            psfile
                        };
                        int result = GSDLL.Execute(args);

                        Microsoft.VisualBasic.FileIO.TextFieldParser tfp = new Microsoft.VisualBasic.FileIO.TextFieldParser(inkfile);
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
                                color_pages++;
                            }
                            else if (ink[3] > 0.0)
                            {
                                mono_pages++;
                            }
                            else
                            {
                                blank_pages++;
                            }
                            pages++;
                        }
                        tfp.Close();
                        try
                        {
                            System.IO.File.Delete(inkfile);
                        }
                        catch
                        {
                            // do nothing
                        }
                    });
                    labelAnalysis.Text = "計 " + pages + " 枚";
                    labelAnalysis.Refresh();

                    DisplayPageAndCost();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("印刷データの解析ができませんでした\r\n\r\n理由：" + ex.Message + "\r\n詳細：" + ex.HResult.ToString(), Properties.Resources.Title);
                    toolStripStatusLabel1.Text = "解析エラー";
                }
                timer1.Enabled = false;
                timer2.Enabled = false;
                statusStrip1.Refresh();
                labelAnalysis.Visible = false;
                labelAnalysis.Refresh();
                this.Refresh();
            }

            this.Activate();
            this.TopMost = true;
            this.Activate();
            this.textUserName.Focus();
            this.Activate();
            this.TopMost = false;

            EnableControls(true);
            if (pages > 0)
            {
                textPassword.Enabled = true;
                textUserName.Enabled = true;
                buttonPrint.Enabled = true;
            }
        }

        private void drawImageFromFile(string fileName)
        {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(
                    fileName,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read);
                System.Drawing.Image img = System.Drawing.Image.FromStream(fs);
                fs.Close();

                int width = (int)(paperWidth / 254.0 * ((Bitmap)img).HorizontalResolution);
                int height = (int)(paperHeight / 254.0 * ((Bitmap)img).VerticalResolution);

                /*
                Rectangle rect = new Rectangle(0, 0, width, height);
                Bitmap bmp = ((Bitmap)img).Clone(rect, img.PixelFormat);
                img.Dispose();
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }
                pictureBox1.Image = bmp;
                */

                pictureBox1.Image = img;
            }
            catch
            {
                // do nothing
            }
            finally 
            {
                // do nothing
            }
        }

        private async void DisplayPageAndCost()
        {
            if (pages > 0)
            {
                labelPageCount.Text = "計 " + pages.ToString() + " 枚";
                toolStripStatusLabel1.Text =
                    "カラー: " + color_pages.ToString() + " 枚　" +
                    "白黒: " + mono_pages.ToString() + " 枚　" +
                    "ブランク: " + blank_pages.ToString() + " 枚　" +
                    "計: " + pages.ToString() + " 枚";

                int cost = -1;
                try
                {
                    cost = await WebAPI.Quotation(color_pages, mono_pages, blank_pages, comboPaper.SelectedIndex, "", comboPrinter.SelectedItem.ToString());
                }
                catch
                {
                    // do nothing
                }
                if (cost >= 0)
                {
                    toolStripStatusLabel1.Text += "　" + cost.ToString() + " Points";
                }
            }
            else
            {
                toolStripStatusLabel1.Text = "";
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
                labelAnalysis.Left = (pictureBox1.Width - labelAnalysis.Width) / 2;
                labelAnalysis.Top = (pictureBox1.Height - labelAnalysis.Height) / 2;
            }
            catch (Exception ex)
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
                if (inkfile != null && System.IO.File.Exists(inkfile))
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(inkfile);
                    if (System.Text.RegularExpressions.Regex.IsMatch(labelAnalysis.Text, @"\w\s\d+"))
                    {
                        labelAnalysis.Text = System.Text.RegularExpressions.Regex.Replace(labelAnalysis.Text, @"(\w)\s\d+", "$1");
                    }
                    labelAnalysis.Text += " " + (info.Length / 44).ToString();
                    labelAnalysis.Refresh();
                }
            }
            catch
            {
                // do nothing
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox1.Refresh();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                System.IO.Directory.Delete(tmpdir, true);
            }
            catch
            {
            }
            try
            {
                System.IO.File.Delete(inkfile);
            }
            catch
            {
            }
            try
            {
                if (!testFile)
                {
                    System.IO.File.Delete(psfile);
                }
            }
            catch
            {
            }
            try
            {
                if (!testFile)
                {
                    System.IO.File.Delete(inifile);
                }
            }
            catch
            {
            }
        }

        private async void buttonPrint_Click(object sender, EventArgs e)
        {
            bool duplexSelectable = comboDuplex.Enabled;
            bool colorSelectable = comboColor.Enabled;

            EnableControls(false);
            string statusText = toolStripStatusLabel1.Text;

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var pwd = new char[32];
            var random = new Random();
            for (int i = 0; i < pwd.Length; i++)
            {
                pwd[i] = chars[random.Next(chars.Length)];
            }
            string name = new string(pwd);

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

                System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                ps.PrinterName = printer;
                if (!ps.IsValid)
                {
                    throw new Exception("選択されたプリンタは有効なプリンタではないようです");
                }
                bool supported_paper = false;
                for (int i=0; i<ps.PaperSizes.Count; i++)
                {
                    if (ps.PaperSizes[i].RawKind == comboPaper.SelectedIndex)
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

                WebAPI.PrintInfo info = await WebAPI.Payment(textUserName.Text, textPassword.Text, color_pages, mono_pages, blank_pages, comboPaper.SelectedIndex, "", comboPrinter.SelectedItem.ToString());
                if (info == null) 
                {
                    throw new Exception("ネットワークエラーです。接続を確認してください。");
                }
                if (info.result != "OK")
                {
                    throw new Exception(info.message);
                }
                labelAnalysis.Text = info.message.Replace("\\n", "\r\n");
                labelAnalysis.Visible = true;


                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                {
                    RegistryKey vendorKey = baseKey.CreateSubKey(@"SOFTWARE\" + Properties.Resources.Vendor, true, RegistryOptions.Volatile);
                    RegistryKey productKey = vendorKey.CreateSubKey(Properties.Resources.Product, true, RegistryOptions.Volatile);

                    productKey.SetValue(Properties.Resources.RegValuePrinter, printer);
                    string[] names = (string[])productKey.GetValue(Properties.Resources.RegValueDocumentNames, new string[] { });

                    bool hit = false;
                    foreach (string s in names)
                    {
                        if (s ==  name)
                        {
                            hit = true;
                            break;
                        }
                    }
                    if (!hit)
                    {
                        Array.Resize(ref names, names.Length + 1);
                        names[names.Length - 1] = name;
                    }

                    productKey.SetValue(Properties.Resources.RegValueDocumentNames, names, RegistryValueKind.MultiString);

                    productKey.Close();
                    vendorKey.Close();
                }

                toolStripStatusLabel1.Text = "印刷中...";
                statusStrip1.Refresh();
                try
                {
                    string duplex = "";
                    if (duplexSelectable && comboDuplex.SelectedIndex == DOUBLE_SIDE)
                    {
                        duplex = "/Duplex true /Tumble false";
                    }

                    string color1 = "/BitsPerPixel 24 ";
                    string color2 = "/Color 2 ";
                    if (colorSelectable && comboColor.SelectedIndex == MONO)
                    {
                        color1 = "/BitsPerPixel 1 ";
                        color2 = "/Color 1 ";
                    }

                    string paper = "/Paper 9 ";
                    if (comboPaper.SelectedIndex > 0)
                    {
                        paper = "/Paper " + Convert.ToInt32(comboPaper.SelectedIndex) + " ";
                    }

                    string[] args = {
                        "gs",
                        "-c",
                        "mark " + 
                            "/NoCancel true " + 
                            "/OutputFile (%printer%" + (printer == null ? "" : printer) + ") " + 
                            color1 +
                            "/UserSettings " +
                            "<<" +
                                "/DocumentName (" + name + ") " + 
                                duplex + 
                                paper +
                                "/Orientation 1 " +
                                color2 +
                            ">> " + 
                            "(mswinpr2) finddevice " + 
                            "putdeviceprops " + 
                            "setdevice",
                        "-dBATCH",
                        "-dNOPAUSE",
                        "-dSAFER",
                        "-f",
                        psfile
                    };
                    int result = GSDLL.Execute(args);

                    if (!testFile)
                    {
                        System.IO.File.Delete(psfile);
                        System.IO.File.Delete(inifile);
                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = "印刷時エラー";
                    statusStrip1.Refresh();
                    throw new Exception("印刷処理ができませんでした\r\n\r\n理由：" + ex.Message + "\r\n詳細：" + ex.HResult.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                EnableControls(true);
                toolStripStatusLabel1.Text = statusText;
            }
        }

        private void comboPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
            ps.PrinterName = comboPrinter.SelectedItem.ToString();
            if (ps.IsValid)
            {
                if (ps.CanDuplex)
                {
                    comboDuplex.Enabled = true;
                }
                else
                {
                    if (comboDuplex.Items.Count > 0)
                    {
                        comboDuplex.SelectedIndex = SINGLE_SIDE;
                    }
                    comboDuplex.Enabled = false;

                }
                if (ps.SupportsColor)
                {
                    comboColor.Enabled = true;
                }
                else
                {
                    comboColor.Enabled = false;
                }
                DisplayPageAndCost();
            }
        }
    }

    class WebAPI
    {
        /*
         * get printing cost
         * POST {baseurl}/api/v1/mps/quotation
         * 
         * print
         * POST {baseurl}/api/v1/mps/payment
         */

        private static HttpClient client = new HttpClient();

        [DataContract]
        internal class PrintInfo
        {
            [DataMember] public string cost { get; set; }
            [DataMember] public string points { get; set; }
            [DataMember] public string result { get; set; }
            [DataMember] public string message { get; set; }
        }

        public static async Task<int> Quotation(int color_count, int mono_count, int blank_count, int paper_size, string room_name, string printer_name)
        {
            int cost = -1;

            string url = (string)Microsoft.Win32.Registry.GetValue(Properties.Resources.RegKeyMariaPrintSystem, Properties.Resources.RegValueBaseUrl, null);
            if (url == null)
            {
                return -1;
            }
            url += "/api/v1/mps/quotation";

            PrintInfo info = await ExecutePost(url, "", "", color_count, mono_count, blank_count, paper_size, room_name, printer_name);

            if (info != null)
            {
                if (!Int32.TryParse(info.cost, out cost))
                {
                    cost = -1;
                }
            }

            return cost;
        }

        public static async Task<PrintInfo> Payment(string username, string password, int color_count, int mono_count, int blank_count, int paper_size, string room_name, string printer_name)
        {
            string url = (string)Microsoft.Win32.Registry.GetValue(Properties.Resources.RegKeyMariaPrintSystem, Properties.Resources.RegValueBaseUrl, null);
            if (url == null)
            {
                return null;
            }
            url += "/api/v1/mps/payment";

            PrintInfo info = await ExecutePost(url, username, password, color_count, mono_count, blank_count, paper_size, room_name, printer_name);

            return info;
        }

        private static async Task<PrintInfo> ExecutePost(string url, string username, string password, int color_count, int mono_count, int blank_count, int paper_size, string room_name, string printer_name)
        {
            PrintInfo info = null;

            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>()
                {
                    { "username", username },
                    { "password", password },
                    { "color_count", color_count.ToString() },
                    { "mono_count", mono_count.ToString() },
                    { "blank_count", blank_count.ToString() },
                    { "paper_size", paper_size.ToString() },
                    { "room_name", room_name },
                    { "printer_name", printer_name },
                };
                FormUrlEncodedContent content = new FormUrlEncodedContent(param);
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(PrintInfo));
                    using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        info = (PrintInfo)serializer.ReadObject(ms);
                    }
                }
            }
            catch
            {
                // do nothing
            }

            return info;
        }
    }

    class INIFILE
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        public static string GetValue(string section, string key, string default_value, string inifile)
        {
            StringBuilder sb = new StringBuilder(1024);

            GetPrivateProfileString(section, key, default_value, sb, Convert.ToUInt32(sb.Capacity), inifile);

            return sb.ToString();
        }
    }

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
                catch (Exception ex)
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

    class GSDLL
    {
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
