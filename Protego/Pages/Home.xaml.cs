using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
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
using System.Windows.Threading;
using Protego.Class;
using System.Timers;
using Protego.UserControls;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        PerformanceCounter perfCPU = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        
        public Home()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();

            //GetOSInfo();
            ProcessorFamily();
            
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            CPU.Value = (int) perfCPU.NextValue();
            CPUpercent.Text = "CPU : " + " " + CPU.Value.ToString() + " " + "%";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
        }
       


       /*
       private void GetOSInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                int systemSku = Convert.ToInt16(provider["Family"]);
                LblSystemSku.Text = "System Sku :" + " " + systemSku.ToString();

                if (systemSku == 198)
                {
                    LblProcFamily.Text = "Family :" + " " + "Intel(R) Core(TM) i7 processor";
                }
            }
        } */
        private void ProcessorFamily()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                int ProcFamily = Convert.ToInt16(provider["Family"]);
                LblProcFamily.Text = ProcFamily.ToString();

                if (ProcFamily == 198)
                {
                    LblProcFamily.Text = "Intel(R) Core(TM) i7 processor";
                }
                if (ProcFamily == 07)
                {
                    LblProcFamily.Text = "AMD Ryzen 5 5600G";
                }
            }

        }

       
    }
}
