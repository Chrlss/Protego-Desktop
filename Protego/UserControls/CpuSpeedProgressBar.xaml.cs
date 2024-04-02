using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Protego.UserControls
{
    public partial class CpuSpeedProgressBar : UserControl
    {
        private PerformanceCounter cpuCounter;

        public CpuSpeedProgressBar()
        {
            InitializeComponent();

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Dispose of the PerformanceCounter when the control is unloaded
            Unloaded += (sender, e) => cpuCounter.Dispose();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double cpuUsage = Math.Min(Math.Max(cpuCounter.NextValue(), 0), 100);

            // Update UI on the UI thread
            Dispatcher.Invoke(() =>
            {
                cpuUsageProgressBar.Value = (int)cpuUsage;
            });
        }
    }
}
