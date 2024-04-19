using System;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Protego.UserControls
{
    /// <summary>
    /// Interaction logic for RAMUsage.xaml
    /// </summary>
    public partial class RAMUsage : UserControl
    {
        private PerformanceCounter ramCounter;

        public RAMUsage()
        {
            InitializeComponent();

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            // Start the timer to update RAM usage periodically
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1); // Update every second
            timer.Start();

            // Initial update of RAM usage
            UpdateRAMUsage();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => UpdateRAMUsage());
        }

        private void UpdateRAMUsage()
        {
            double availableRamMB = ramCounter.NextValue() / 1024.0;
            double totalRamMB = GetTotalRamMB();
            double usedRamMB = totalRamMB - availableRamMB;

            Dispatcher.Invoke(() =>
            {
                ramUsageTextBlock.Text = $"{usedRamMB.ToString("0.0", CultureInfo.InvariantCulture)} / {totalRamMB.ToString("0.0", CultureInfo.InvariantCulture)} GB";
            });
        }

        private double GetTotalRamMB()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (var item in searcher.Get())
                {
                    var totalPhysicalMemory = Convert.ToDouble(item["TotalPhysicalMemory"]);
                    return totalPhysicalMemory / (1024.0 * 1024.0 * 1024.0); // Convert bytes to GB directly
                }
            }
            return 0;
        }
    }
}
