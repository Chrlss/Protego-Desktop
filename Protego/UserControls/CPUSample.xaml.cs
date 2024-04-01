using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Protego.UserControls
{
    /// <summary>
    /// Interaction logic for CPUSample.xaml
    /// </summary>
    public partial class CPUSample : UserControl
    {
        private PerformanceCounter cpuCounter;
        public CPUSample()
        {
            InitializeComponent();

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            Task.Run(() => InitializeCounter());
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private async void InitializeCounter()
        {
            await Task.Delay(1000);
            cpuCounter.NextValue();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            double cpuUsage = Math.Round(cpuCounter.NextValue(), 2);
            CPUpercent.Text = $"{cpuUsage}%";

        }
    }
}
