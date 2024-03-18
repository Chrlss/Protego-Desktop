using Protego.UserControls;
using System.Diagnostics;
using System.Management;
using System.Text;
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
            Task.Run(() => ProcessorFamily());
            GetTotalRam();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            RAM.Value = (int)perfRAM.NextValue();
            RAMpercent.Text = RAM.Value.ToString() + "%";
            Task.Run(() => ProcessorFamily());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
        }
        private void GetTotalRam()
        {
            try
            {
                // Get total RAM using WMI
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\cimv2", "SELECT Capacity FROM Win32_PhysicalMemory");
                ulong totalRam = 0;
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    totalRam += (ulong)queryObj["Capacity"];
                }

                // Convert to GB for display (cast to double for accurate division)
                double ramInGb = Math.Round((double)totalRam / (1024 * 1024 * 1024), 2);

                // Update progress bar value (assuming maximum RAM is 16 GB)
                ramProgressBar.Maximum = 16384; // Adjust based on your expected maximum RAM
                ramProgressBar.Value = ramInGb;

                ramTextBlock.Text = $"Total RAM: {ramInGb} GB";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving RAM information: {ex.Message}");
            }
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
            StringBuilder sbFamily = new StringBuilder();

            foreach (var provider in providers)
            {
                int ProcFamily = Convert.ToInt16(provider["Family"]);
                
                if (ProcFamily == 198)
                {
                    sbFamily.Append("Intel(R) Core(TM) i7 processor");
                }
                if (ProcFamily == 107)
                {
                    sbFamily.Append("AMD Ryzen 5 5600G");
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                LblProcFamily.Text = sbFamily.ToString();
            });

        }

       
    }
}
