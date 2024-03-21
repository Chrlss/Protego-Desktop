using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Protego.UserControls
{
    /// <summary>
    /// Interaction logic for CpuSpeedProgressBar.xaml
    /// </summary>
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

            Task.Run(() => InitializeCounter());
        }
        private async void InitializeCounter()
        {
            await Task.Delay(1000);
            cpuCounter.NextValue();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            double cpuUsage = Math.Min(Math.Max(cpuCounter.NextValue(), 0), 100);
            cpuUsageProgressBar.Value = (int)cpuUsage;
            
        }
    }
}
