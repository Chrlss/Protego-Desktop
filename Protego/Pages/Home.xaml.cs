using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Protego.UserControls;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private readonly PerformanceCounter perfRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public Home()
        {
            InitializeComponent();
            InitializeTimer();
            LoadProcessorFamilyAsync();
            UpdateScanCountLabel();

        }

        private void InitializeTimer()
        {
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            RAM.Value = (int)perfRAM.NextValue();
            RAMpercent.Text = $"{RAM.Value}%";
            LoadProcessorFamilyAsync();
        }


        private void UpdateScanCountLabel()
        {
            FlashDriveScanCountLabel.Text = $"{Protection.FlashDriveScanCount}";
        }

        private async void LoadProcessorFamilyAsync()
        {
            try
            {
                var processorFamily = await Task.Run(GetProcessorFamily);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LblProcFamily.Text = processorFamily;
                });
            }
            catch (Exception ex)
            {
                //display error information
                string errorMessage = $"An error occurred while loading processor family: {ex.Message}";
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string GetProcessorFamily()
        {
            // Dictionary to map processor family codes to their names
            var familyMapping = new Dictionary<int, string>
            {
                {198, "Intel(R) Core(TM) i7"},
                {107, "AMD Ryzen 5 5600G"},
                {11, "Pentium(R) brand"}
            };

            ManagementClass wmi = new ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {                
                int procFamily = Convert.ToInt16(provider["Family"]);
                // Check if the code exists in the dictionary
                if (familyMapping.ContainsKey(procFamily))
                {
                    return familyMapping[procFamily]; // Return the corresponding family name
                }
            }
            return "Unknown Processor Family";
        }


    }
}
