using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Protego.UserControls
{
    public partial class CPUSample : UserControl
    {
        private PerformanceCounter cpuCounter;
        private DispatcherTimer timer;

        public CPUSample()
        {
            InitializeComponent();

            InitializeCounter();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            Unloaded += CPUSample_Unloaded; // Register the Unloaded event handler
        }

        private void InitializeCounter()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // Discard the initial value
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double cpuUsage = Math.Round(cpuCounter.NextValue(), 2);

            // Update UI on the UI thread
            Dispatcher.Invoke(() =>
            {
                CPUpercent.Text = $"{cpuUsage}%";
            });
        }

        private void CPUSample_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Dispose of the PerformanceCounter and stop the timer when the control is unloaded
            cpuCounter?.Dispose();
            timer?.Stop();
            timer.Tick -= Timer_Tick;
        }
    }
}
