using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Monitor.xaml
    /// </summary>
    public partial class Monitor : Page
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public Monitor()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0);
            timer.Start();

            GetOSInfo();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            GetTime();
        }
        private void GetTime()
        {
            DateTime date;
            date = DateTime.Now;
            LblClock.Text = date.ToLongDateString() + "   " + date.ToLongTimeString();
        }
        private void GetOSInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                string procStatus = provider["Status"].ToString();
                int procFamily = Convert.ToInt16(provider["Family"]);

                LblStatus.Text = "Processor Status" + " " + procStatus.ToString();

                if (procFamily == 107)
                {
                    LblSku.Text = "AMD Ryzen 5 5600G";
                }

                if (procFamily == 11)
                {
                    LblSku.Text = "Pentium(R) brand ";
                }

                if (procFamily == 12)
                {
                    LblSku.Text = "Pentium(R) Pro ";
                }

                if (procFamily == 13)
                {
                    LblSku.Text = "Pentium(R) II ";

                }

                if (procFamily == 14)
                {
                    LblSku.Text = "Pentium(R) processor with MMX(TM) technology";
                }

                if (procFamily == 14)
                {
                    LblSku.Text = "Pentium(R) processor with MMX(TM) technology";
                }

                if (procFamily == 15)
                {
                    LblSku.Text = "Celeron(TM)";
                }

                if (procFamily == 16)
                {
                    LblSku.Text = "Pentium(R) II Xeon(TM) ";
                }
            }
        }
    }
}
