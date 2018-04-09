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

namespace MariaPrintManager
{
    public partial class MainForm : Form
    {
        string psfile = null;
        string tmpdir = null;
        int pages = 0;
        int unitPrice = 10;
        bool testFIle = false;
        int[,] A = new int[,] {
	        {  841, 1189 },     // A0
	        {  594,  841 },     // A1
	        {  420,  594 },     // A2
	        {  297,  420 },     // A3
	        {  210,  297 },     // A4
	        {  148,  210 },     // A5
	        {  105,  148 },     // A6
	        {   74,  105 },     // A7
	        {   52,   74 },     // A8
	        {   37,   52 },     // A9
	        {   26,   37 }      // A10
        };
        int[,] B = new int[,] {
	        { 1030, 1456 },     // B0
	        {  728, 1030 },     // B1
	        {  515,  728 },     // B2
	        {  364,  515 },     // B3
	        {  257,  364 },     // B4
	        {  182,  257 },     // B5
	        {  128,  182 },     // B6
	        {   91,  128 },     // B7
	        {   64,   91 },     // B8
	        {   45,   64 },     // B9
	        {   32,   45 }      // B10
        };

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
                testFIle = true;
            }
            tmpdir = psfile + ".extract";

            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                ManagementObjectCollection moc = mos.Get();
                foreach (ManagementObject mo in moc)
                {
                    string printproc = mo["PrintProcessor"].ToString();
                    if (printproc.Equals("mariaprint", StringComparison.OrdinalIgnoreCase))
                    {
                        comboPrinter.Items.Add(mo["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
            }
            if (comboPrinter.Items.Count > 0)
            {
                comboPrinter.SelectedIndex = 0;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (psfile != null && System.IO.File.Exists(psfile))
            {
                this.Refresh();
                toolStripStatusLabel1.Text = "ファイル解析中";
                statusStrip1.Refresh();
                try
                {
                    System.IO.Directory.CreateDirectory(tmpdir);
                    string output = System.IO.Path.Combine(tmpdir, "def-%04d.png");
                    string[] args = {
                        "gs",
                        "-dBATCH",
                        "-dNOPAUSE",
                        "-dSAFER",
                        "-sDEVICE=png16m",
                        "-sOutputFile=" + output,
                        "-dUseMediaBox",
                        "-sPAPERSIZE=a4",
                        "-dTextAlphaBits=4",
                        "-dGraphicsAlphaBits=4",
                        "-dDownScaleFactor=4",
                        "-r150",
                        "-f",
                        psfile
                    };
                    int result = GSDLL.Execute(args);

                    pages = System.IO.Directory.GetFiles(tmpdir, "*", System.IO.SearchOption.TopDirectoryOnly).Length;
                    if (pages > 0)
                    {
                        labelPageCount.Text = "計: " + pages.ToString() + " 枚";
                        drawImageFromFile(System.IO.Path.Combine(tmpdir, "def-0001.png"));
                        textPassword.Enabled = true;
                        textUserName.Enabled = true;
                        buttonPrint.Enabled = true;
                        toolStripStatusLabel1.Text = "計 " + pages.ToString() + " 枚 / " + (unitPrice * pages).ToString() + " ポイント";
                    }
                    else
                    {
                        toolStripStatusLabel1.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("印刷処理ができませんでした\r\n\r\n理由：" + ex.Message + "\r\n詳細：" + ex.HResult.ToString(), Properties.Resources.Title);
                    toolStripStatusLabel1.Text = "解析エラー";
                }
                statusStrip1.Refresh();
            }
            this.Activate();
            this.TopMost = true;
            this.Activate();
            this.textUserName.Focus();
            this.Activate();
            this.TopMost = false;
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

                // A4: width=210mm, height=297mm
                Rectangle rect = new Rectangle(
                    0, 0,
                    (int)(A[4, 0] / 25.4 * ((Bitmap)img).HorizontalResolution),
                    (int)(A[4, 1] / 25.4 * ((Bitmap)img).VerticalResolution));
                Bitmap bmp = ((Bitmap)img).Clone(rect, img.PixelFormat);
                img.Dispose();
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }
                pictureBox1.Image = bmp;
            }
            finally 
            {
                // do nothing
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
            catch (Exception ex)
            {
            }
            try
            {
                if (!testFIle)
                {
                    System.IO.File.Delete(psfile);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            textPassword.Enabled = false;
            textUserName.Enabled = false;
            buttonPrint.Enabled = false;

            string printer = null;
            if (comboPrinter.SelectedIndex >= 0)
            {
                printer = comboPrinter.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("プリンタが選択されていません");
                return;
            }
            /*
            foreach (string s in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
            }
            */
            /*
            try
            {
                printer = (string)Microsoft.Win32.Registry.GetValue(Properties.Resources.RegKeyMariaPrintSystem, Properties.Resources.RegValuePrinter, null);
            }
            catch (Exception ex)
            {
                printer = null;
            }
            if (printer == null)
            {
                try
                {
                    ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                    ManagementObjectCollection moc = mos.Get();
                    foreach (ManagementObject mo in moc)
                    {
                        string port = mo["PortName"].ToString();
                        if (port.StartsWith("IP") || port.StartsWith("1"))
                        {
                            printer = mo["Name"].ToString();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    printer = null;
                }
            }
            */

            toolStripStatusLabel1.Text = "印刷中...";
            statusStrip1.Refresh();
            try
            {
                string[] args = {
                    "gs",
                    "-dBATCH",
                    "-dNOPAUSE",
                    "-dSAFER",
                    "-dNoCancel",
                    "-sDEVICE=mswinpr2",
                    "-sOutputFile=%printer%" + (printer == null ? "" : printer),
                    "-sPAPERSIZE=a4",
                    "-f",
                    psfile
                };
                int result = GSDLL.Execute(args);

                if (!testFIle)
                {
                    System.IO.File.Delete(psfile);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("印刷処理ができませんでした\r\n\r\n理由：" + ex.Message + "\r\n詳細：" + ex.HResult.ToString(), Properties.Resources.Title);
                toolStripStatusLabel1.Text = "印刷時エラー";
            }
            textPassword.Enabled = true;
            textUserName.Enabled = true;
            buttonPrint.Enabled = true;
        }
    }

    class GSDLL32
    {
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        public static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

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
    }

    class GSDLL64
    {
        [DllImport("gsdll64.dll", EntryPoint = "gsapi_new_instance")]
        public static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

        [DllImport("gsdll64.dll", EntryPoint = "gsapi_init_with_args")]
        public static extern int gsapi_init_with_args(IntPtr instance, int argc, string[] argv);

        [DllImport("gsdll64.dll", EntryPoint = "gsapi_exit")]
        public static extern int gsapi_exit(IntPtr instance);

        [DllImport("gsdll64.dll", EntryPoint = "gsapi_delete_instance")]
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
