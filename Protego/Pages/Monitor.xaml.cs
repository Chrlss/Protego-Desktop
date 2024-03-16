using System;
using System.Collections.Generic;
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
using System.Management;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Monitor.xaml
    /// </summary>
    public partial class Monitor : Page
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public Monitor()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0);
            timer.Start();

            // Start the methods on separate threads
            Task.Run(() => GetOSInfo());
            Task.Run(() => GetRamInfo());
            Task.Run(() => GetStorage());
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => GetOSInfo());
        }
        private void GetOSInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();
            StringBuilder sb = new StringBuilder();
            foreach (var provider in providers)
            {
                int clock = Convert.ToInt32(provider["MaxClockSpeed"]);
                int procFamily = Convert.ToInt16(provider["Family"]);

                sb.AppendLine($"{clock} MHz");

                if (procFamily == 107)
                {
                    sb.Append("AMD Ryzen 5 5600G");
                }

                // Add more conditions for other processor families

            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                LblProcie.Text = sb.ToString();
            });
            
        }
        private void GetRamInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_ComputerSystem");
            var system = wmi.GetInstances().Cast<System.Management.ManagementObject>().FirstOrDefault();
            if (system != null)
            {
                long totalPhysicalMemory = Convert.ToInt64(system["TotalPhysicalMemory"]);

                // Use the Dispatcher to marshal the UI update back to the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LblTotalRam.Text = $"{totalPhysicalMemory / (1024 * 1024), 2} MB";
                });
            }

            wmi = new System.Management.ManagementClass("Win32_PhysicalMemory");
            var modules = wmi.GetInstances();
            StringBuilder sb = new StringBuilder();
            foreach (var module in modules)
            {
                int speed = Convert.ToInt32(module["Speed"]);
                string bankLabel = module["BankLabel"].ToString();

                // Use the Dispatcher to marshal the UI update back to the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    sb.Append(String.Concat(bankLabel, ": ", speed, " MHz\n"));
                    LblRamSpeed.Text = sb.ToString();
                });
            }
        }
        private void GetStorage()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_LogicalDisk");
            var providers = wmi.GetInstances();

            ulong maxFreeSpace = 0;
            ulong totalSize = 0;
            string maxDrive = "";

            foreach (var provider in providers)
            {
                string drive = provider["DeviceID"].ToString();
                string type = provider["DriveType"].ToString();
                ulong sizeInBytes = Convert.ToUInt64(provider["Size"]);
                ulong freeSpaceInBytes = Convert.ToUInt64(provider["FreeSpace"]);

                totalSize += sizeInBytes;

                if (freeSpaceInBytes > maxFreeSpace)
                {
                    maxFreeSpace = freeSpaceInBytes;
                    maxDrive = drive;
                }
            }

            double totalSizeInGB = Math.Round(Convert.ToDouble(totalSize) / (1024 * 1024 * 1024), 2);

            // Use the Dispatcher to marshal the UI update back to the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                LblTotalStorage.Text = "Total Storage: " + totalSizeInGB.ToString() + " GB";
            });

            if (maxDrive != "")
            {
                var provider = new ManagementObject("Win32_LogicalDisk.DeviceID='" + maxDrive + "'");
                ulong sizeInBytes = Convert.ToUInt64(provider["Size"]);
                ulong freeSpaceInBytes = Convert.ToUInt64(provider["FreeSpace"]);
                double freeSpaceInGB = Math.Round(Convert.ToDouble(freeSpaceInBytes) / (1024 * 1024 * 1024), 2);

                // Use the Dispatcher to marshal the UI update back to the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LblFreeStorage.Text = maxDrive + " (" + freeSpaceInGB.ToString() + " GB)";
                });
            }
            else
            {
                // Use the Dispatcher to marshal the UI update back to the UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LblFreeStorage.Text = "No storage devices found.";
                });
            }
        }
    }
}