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
        FileSystemWatcher watcher1 = new FileSystemWatcher();
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

            cleanup();
        }

        ~Watcher()
        {
            cleanup();
        }

        private void deleteDirectries(string path, string name)
        {
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                System.IO.DirectoryInfo[] dirs = di.GetDirectories(name, System.IO.SearchOption.TopDirectoryOnly);
                foreach (System.IO.DirectoryInfo d in dirs)
                {
                    try
                    {
                        d.Delete(true);
                    }
                    catch
                    {
                        //
                    }
                }
            }
            catch
            {
                //
            }
        }

        private void deleteFiles(string path, string name)
        {
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                System.IO.FileInfo[] files = di.GetFiles(name, System.IO.SearchOption.TopDirectoryOnly);
                foreach (System.IO.FileInfo f in files)
                {
                    try
                    {
                        f.Delete();
                    }
                    catch
                    {
                        //
                    }
                }
            }
            catch
            {
                //
            }
        }

        private void cleanup()
        {

            deleteDirectries(watcher1.Path, "MR_*.extract");
            deleteFiles(watcher1.Path, "MR_*.ink");
            deleteFiles(watcher1.Path, "MR_*.ini");
            deleteFiles(watcher1.Path, "MR_*.ps");
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
            bool newMutex;

            System.Threading.Mutex mutex = new System.Threading.Mutex(true, "MariaPrintTray", out newMutex);

            if (newMutex == false)
            {
                mutex.Close();
                return;
            }

            try
            {
                SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Watcher watcher1 = new Watcher();

                Application.Run();
            }
            finally
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        [System.Runtime.InteropServices.DllImport("SHCore.dll")]
        private static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT dpiFlag);

        private enum PROCESS_DPI_AWARENESS
        {
            Process_DPI_Unaware = 0,
            Process_System_DPI_Aware = 1,
            Process_Per_Monitor_DPI_Aware = 2
        }
        private enum DPI_AWARENESS_CONTEXT
        {
            DPI_AWARENESS_CONTEXT_UNAWARE = 16,
            DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = 17,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = 18,
            DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = 34
        }
    }
}
