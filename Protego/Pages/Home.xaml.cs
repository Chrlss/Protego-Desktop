using Protego.UserControls;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using LibreHardwareMonitor.Hardware;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        PerformanceCounter perfRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
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
            RAM.Value = (int)perfRAM.NextValue();
            RAMpercent.Text = RAM.Value.ToString() + "%";
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
