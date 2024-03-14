using System;
using System.Collections.Generic;
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

namespace Protego.UserControls
{
    /// <summary>
    /// Interaction logic for CPU1.xaml
    /// </summary>
    public partial class CPU1 : UserControl
    {
        
        public CPU1()
        {
            InitializeComponent();
            

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
               GetAndDisplayCPUClockSpeed();
            };
            timer.Start();
        }
        private void GetAndDisplayCPUClockSpeed()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (ManagementObject obj in searcher.Get())
                {
                    // Get the CPU clock speed in MHz
                    uint clockSpeedMHz = (uint)obj["MaxClockSpeed"];

                    // Normalize the clock speed to a value between 0 and 100 for the progress bar
                    double normalizedClockSpeed = (double)clockSpeedMHz / 2592; // Assuming a maximum clock speed of 5000 MHz

                    // Update the progress bar
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        cpuClockProgressBar.Value = normalizedClockSpeed * 100; // Convert to percentage
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

       
    }
}
