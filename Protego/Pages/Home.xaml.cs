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
        }

        private void InitializeTimer()
        {
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private async void LoadProcessorFamilyAsync()
        {
            try
            {
                string processorFamily = await Task.Run(() => GetProcessorFamily());
                Application.Current.Dispatcher.Invoke(() => LblProcFamily.Text = processorFamily);
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, such as logging or displaying an error message.
                MessageBox.Show($"An error occurred while loading processor family: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetProcessorFamily()
        {
            ManagementClass wmi = new ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();
            StringBuilder sbFamily = new StringBuilder();

            foreach (var provider in providers)
            {
                int procFamily = Convert.ToInt16(provider["Family"]);

                if (procFamily == 198)
                {
                    sbFamily.Append("Intel(R) Core(TM) i7");
                }
                else if (procFamily == 107)
                {
                    sbFamily.Append("AMD Ryzen 5 5600G");
                }
                else if (procFamily == 11)
                {
                    sbFamily.Append("Pentium(R) brand");
                }
            }

            return sbFamily.ToString();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RAM.Value = (int)perfRAM.NextValue();
            RAMpercent.Text = $"{RAM.Value}%";
            LoadProcessorFamilyAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
        }
    }
}
