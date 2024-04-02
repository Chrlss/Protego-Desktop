using System.Diagnostics;
using System.Globalization;
using System.Management;
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
            Task.Run(() => GetRAMUsage());
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1); // 1 second
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => GetRAMUsage());
        }

        private void GetRAMUsage()
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
                    return totalPhysicalMemory / 1024.0 / 1024.0 / 1024.0; // Convert bytes to MB
                }
            }
            return 0;
        }

    }
}