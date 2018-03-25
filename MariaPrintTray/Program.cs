using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace MariaPrintTray
{
    class Watcher
    {
        private NotifyIcon notify1;

        public Watcher()
        {
            string path = null;
            try
            {
                path = (string)Microsoft.Win32.Registry.GetValue(Properties.Resources.RegKeyPortMonitorSettings, Properties.Resources.RegValueDirectory, null);
            }
            catch (Exception ex)
            {
                path = null;
            }
            if (path == null)
            {
                path = System.Environment.GetEnvironmentVariable("TEMP");
            }
            FileSystemWatcher watcher1 = new FileSystemWatcher();
            watcher1.Path = path;
            watcher1.NotifyFilter = NotifyFilters.FileName;
            watcher1.Filter = @"*.ps";
            watcher1.EnableRaisingEvents = true;
            watcher1.Created += new FileSystemEventHandler(watcher1_Created);
            watcher1.Renamed += new RenamedEventHandler(watcher1_Created);

            ContextMenuStrip menu1 = new ContextMenuStrip();
            ToolStripMenuItem menuItemAbout = new ToolStripMenuItem();
            ToolStripMenuItem menuItemExit = new ToolStripMenuItem();
            menu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {menuItemAbout, menuItemExit});
            menuItemAbout.Text = Properties.Resources.MenuTextAbout;
            menuItemAbout.Click += new System.EventHandler(menuItemAbout_Click);
            menuItemExit.Text = Properties.Resources.MenuTextExit;
            menuItemExit.Click += new System.EventHandler(menuItemExit_Click);

            notify1 = new NotifyIcon();
            notify1.Icon = Properties.Resources.Icon1;
            notify1.Text = Properties.Resources.Title;
            notify1.Visible = true;
            notify1.ContextMenuStrip = menu1;
        }

        ~Watcher()
        {
        }

        private void watcher1_Created(object sender, FileSystemEventArgs e)
        {
            string psFileName = e.FullPath;
            string dirName = Path.GetDirectoryName(e.FullPath);

            string gsshell = null;
            try
            {
                gsshell = (string)Microsoft.Win32.Registry.GetValue(Properties.Resources.RegKeyMariaPrintSystem, Properties.Resources.RegValuePsShell, null);
            }
            catch (Exception ex)
            {
                gsshell = null;
            }
            if (gsshell != null && gsshell != "")
            {
                gsshell = System.IO.Path.Combine(Application.StartupPath, gsshell);
                if (System.IO.File.Exists(gsshell))
                {
                    try
                    {
                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                        psi.FileName = gsshell;
                        psi.Arguments = System.IO.Path.Combine(dirName, psFileName);
                        System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Resources.Title + "\r\n\r\n" + Properties.Resources.AutherCredit + "\r\n" + Properties.Resources.IconCredit);
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            notify1.Visible = false;
            Application.Exit();
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Watcher watcher1 = new Watcher();

            Application.Run();
        }

    }
}
