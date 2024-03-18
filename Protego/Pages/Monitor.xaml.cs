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
using System.Diagnostics;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Monitor.xaml
    /// </summary>
    public partial class Monitor : Page
    {
        PerformanceCounter perfRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public Monitor()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();

            // Start the methods on separate threads
            Task.Run(() => GetOSInfo());
            Task.Run(() => GetRamInfo());
            Task.Run(() => GetStorage());

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RAM.Value = (int)perfRAM.NextValue();
            RAMpercent.Text = RAM.Value.ToString() + "%";
            Task.Run(() => GetOSInfo());
        }
        private void GetOSInfo()
        {
            if (LblProcie == null || LblClock == null)
            {
                // Initialize LblProcie and LblClock if they are null
                LblProcie = new TextBlock();
                LblClock = new TextBlock();
            }

            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();
            StringBuilder sbFamily = new StringBuilder();
            StringBuilder sbClock = new StringBuilder();
            foreach (var provider in providers)
            {
                int clock = Convert.ToInt32(provider["MaxClockSpeed"]);
                int procFamily = Convert.ToInt16(provider["Family"]);

                sbClock.AppendLine($"{clock} MHz");

                if (procFamily == 107)
                {
                    sbFamily.Append("AMD Ryzen 5 5600G");
                }

                if (procFamily == 11)
                {
                    sbFamily.Append("Pentium(R) brand");
                }

                if (procFamily == 12)
                {
                    sbFamily.Append("Pentium(R) Pro");
                }

                if (procFamily == 13)
                {
                    sbFamily.Append("Pentium(R) II");
                }

                if (procFamily == 14)
                {
                    sbFamily.Append("Pentium(R) processor with MMX(TM) technology");
                }

                if (procFamily == 15)
                {
                    sbFamily.Append("Celeron(TM)");
                }

                if (procFamily == 16)
                {
                    sbFamily.Append("Pentium(R) II Xeon(TM)");
                }

                if (procFamily == 17)
                {
                    sbFamily.Append("Pentium(R) III");
                }

                if (procFamily == 20)
                {
                    sbFamily.Append("Intel(R) Celeron(R) M processor");
                }
                // Add more conditions for other processor families

            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                LblProcie.Text = sbFamily.ToString();
                LblClock.Text = sbClock.ToString();
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
            ManagementObjectSearcher diskDriveSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            ManagementObjectCollection diskDrives = diskDriveSearcher.Get();

            ulong totalSize = 0;
            ulong maxFreeSpace = 0;
            string maxDrive = "";

            foreach (ManagementObject diskDrive in diskDrives)
            {
                string deviceId = diskDrive["DeviceID"].ToString();
                ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + deviceId + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
                ManagementObjectCollection partitions = partitionSearcher.Get();

                foreach (ManagementObject partition in partitions)
                {
                    string partitionDeviceId = partition["DeviceID"].ToString();
                    ManagementObjectSearcher logicalDiskSearcher = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partitionDeviceId + "'} WHERE AssocClass = Win32_LogicalDiskToPartition");
                    ManagementObjectCollection logicalDisks = logicalDiskSearcher.Get();

                    foreach (ManagementObject logicalDisk in logicalDisks)
                    {
                        ulong sizeInBytes = Convert.ToUInt64(logicalDisk["Size"]);
                        ulong freeSpaceInBytes = Convert.ToUInt64(logicalDisk["FreeSpace"]);

                        totalSize += sizeInBytes;

                        if (freeSpaceInBytes > maxFreeSpace)
                        {
                            maxFreeSpace = freeSpaceInBytes;
                            maxDrive = logicalDisk["DeviceID"].ToString();
                        }
                    }
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
                }
                );
            }
        }

       
    }
    
}