using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Drawing.Printing;
using System.Printing;

namespace MariaPrintConfigurator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            buttonSave.Enabled = false;
            checkedListBoxPrinters.Enabled = false;
            listBoxPrinters.Enabled = false;
            labelWait.Left = splitContainer1.Left + (splitContainer1.Width - labelWait.Width) / 2;
            labelWait.Top = splitContainer1.Top + (splitContainer1.Panel1.Height - labelWait.Height) / 2;
            labelWait.Visible = true;
            checkedListBoxPrinters.Refresh();
            listBoxPrinters.Refresh();
            labelWait.Refresh();
            this.Refresh();

            PrintServer ps = new PrintServer();
            foreach (object o in checkedListBoxPrinters.Items)
            {
                string proc = "";
                int i = checkedListBoxPrinters.Items.IndexOf(o);
                if (checkedListBoxPrinters.GetItemChecked(i))
                {
                    proc = "mariaprint";
                }
                else
                {
                    proc = "winprint";
                }

                System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher("Select * from Win32_Printer where Name='" + o.ToString() + "'");
                System.Management.ManagementObjectCollection moc = mos.Get();
                foreach (System.Management.ManagementObject printer in moc)
                {
                    printer["PrintProcessor"] = proc;
                    printer.Put();
                    printer.Dispose();
                }
                moc.Dispose();
                mos.Dispose();
            }

            this.UseWaitCursor = false;
            this.Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Activate();
            this.TopMost = true;
            this.Activate();
            this.buttonSave.Focus();
            this.Activate();
            this.TopMost = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkedListBoxPrinters.Width = splitContainer1.Panel1.Width;
            checkedListBoxPrinters.Height = splitContainer1.Panel1.Height - checkedListBoxPrinters.Top;
            listBoxPrinters.Width = splitContainer1.Panel2.Width;
            listBoxPrinters.Height = splitContainer1.Panel2.Height - listBoxPrinters.Top;

            PrintServer ps = new PrintServer();
            PrintQueueCollection pqs = ps.GetPrintQueues();
            foreach (PrintQueue pq in pqs)
            {
                if (pq.IsXpsDevice)
                {
                    listBoxPrinters.Items.Add(pq.Name + " (XPSデバイス)");
                }
                else if (pq.QueuePrintProcessor.Name == "mariaprint")
                {
                    int i = checkedListBoxPrinters.Items.Add(pq.Name);
                    checkedListBoxPrinters.SetItemChecked(i, true);
                }
                /*
                else if (pq.QueuePrintProcessor.Name != "winprint")
                {
                    listBoxPrinters.Items.Add(pq.Name + " (独自プリントプロセッサ)");
                }
                */
                else
                {
                    checkedListBoxPrinters.Items.Add(pq.Name);
                }
            }
        }
    }
}
