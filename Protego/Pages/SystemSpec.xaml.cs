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
    #region
    public partial class SystemSpec : Page
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public SystemSpec()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0);
            timer.Start();

            GetOSInfo();
            GetProcessorInfo();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetTime();            
        }
        private void GetTime()
        {
            DateTime date;
            date = DateTime.Now;
            lbl1.Text = date.ToLongDateString() + "   " + date.ToLongTimeString();
        }
        #endregion

        #region // OS Info
        private void GetOSInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_ComputerSystem");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                string systemSku = provider["SystemSKUNumber"].ToString();
                lbl2.Text = "System Sku :" + " " + systemSku.ToString();
            }
        }
        #endregion

        #region // Processor Info
        private void GetProcessorInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                int procFamily = Convert.ToInt16(provider["Family"]);
                int procSpeed = Convert.ToInt32(provider["CurrentClockSpeed"]);
                string procStatus = provider["Status"].ToString();
                Boolean powerManagementSupported = Convert.ToBoolean(provider["PowerManagementSupported"]);

                lbl3.Text = "Processor Family: " + " " + procFamily.ToString();
                lbl4.Text = "Processor Clock   Speed: " + " " + procSpeed.ToString();
                lbl5.Text = "Processor Status" + " " + procStatus.ToString();
                lbl6.Text = powerManagementSupported.ToString();
            }
        }
        #endregion
    }

}
