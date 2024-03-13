using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
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
using Protego.Class;
using System.Timers;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        PerformanceCounter perfCPU = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
       System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public Home()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            CPU.Value = (int) perfCPU.NextValue();
            CPUpercent.Text = "CPU : " + " " + CPU.Value.ToString() + " " + "%";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
        }
       

        private long GetTotalRAM()
        {
            long totalVisibleBytes = 0;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select Capacity From Win32_PhysicalMemory");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                totalVisibleBytes += Convert.ToInt64(queryObj["Capacity"]);
            }
            return totalVisibleBytes;
        }

        private string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double value = bytes;
            int i = 0;
            while (value >= 1024 && i < units.Length - 1)
            {
                value /= 1024;
                ++i;
            }
            return $"{value:F1}{units[i]}";
        }

        
    }
}
