using System;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows;

namespace Protego.Pages
{
    public partial class Monitor : Page
    {
        PerformanceCounter perfRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        DispatcherTimer timer = new DispatcherTimer();

        public Monitor()
        {
            InitializeComponent();
            InitializeTimer();
            Task.Run(() => GetProcessorFamily());
            Task.Run(() => GetRamInfo());
            Task.Run(() => GetStorage());
            Task.Run(() => GetOSInfo());
            LoadProcessorFamilyAsync();
        }

        private void InitializeTimer()
        {
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateRAMInfo();
            Task.Run(() => GetProcessorFamily());
        }

        private void UpdateRAMInfo()
        {
            RAM.Value = (int)perfRAM.NextValue();
            RAMpercent.Text = RAM.Value.ToString() + "%";
        }

        private async void LoadProcessorFamilyAsync()
        {
            try
            {
                string processorFamily = await Task.Run(() => GetProcessorFamily());
                Application.Current.Dispatcher.Invoke(() => LblProcFamily.Text = processorFamily);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading processor family: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetProcessorFamily()
        {
            ManagementClass wmi = new ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();
            StringBuilder sbFamily = new StringBuilder();

            foreach (var provider in providers)
            {
                int procFamily = Convert.ToInt16(provider["Family"]);

                if (procFamily == 198)
                {
                    sbFamily.Append("Intel(R) Core(TM) i7");
                }
                else if (procFamily == 107)
                {
                    sbFamily.Append("AMD Ryzen 5 5600G");
                }
                else if (procFamily == 11)
                {
                    sbFamily.Append("Pentium(R) brand");
                }
            }

            return sbFamily.ToString();
        }

        private void GetOSInfo()
        {
            StringBuilder sbFamily = new StringBuilder();
            StringBuilder sbClock = new StringBuilder();

            using (ManagementClass wmi = new ManagementClass("Win32_Processor"))
            {
                var providers = wmi.GetInstances();
                foreach (var provider in providers)
                {
                    int clock = Convert.ToInt32(provider["MaxClockSpeed"]);
                    int procFamily = Convert.ToInt16(provider["Family"]);

                    sbClock.AppendLine($"{clock} MHz");

                    switch (procFamily)
                    {
                        case 107:
                            sbFamily.Append("AMD Ryzen 5 5600G");
                            break;
                        case 11:
                            sbFamily.Append("Pentium(R) brand");
                            break;
                        case 12:
                            sbFamily.Append("Pentium(R) Pro");
                            break;
                            // Add more cases for other processor families
                    }
                }
            }

               Application.Current.Dispatcher.Invoke(() =>
            {
                LblProcie.Text = sbFamily.ToString();
                LblClock.Text = sbClock.ToString();
            });
        }

        private void GetRamInfo()
        {
            using (ManagementClass wmi = new ManagementClass("Win32_ComputerSystem"))
            {
                var system = wmi.GetInstances().Cast<ManagementObject>().FirstOrDefault();
                if (system != null)
                {
                    long totalPhysicalMemory = Convert.ToInt64(system["TotalPhysicalMemory"]);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LblTotalRam.Text = $"{totalPhysicalMemory / (1024 * 1024),2} MB";
                    });
                }
            }

            using (ManagementClass wmi = new ManagementClass("Win32_PhysicalMemory"))
            {
                var modules = wmi.GetInstances();
                StringBuilder sb = new StringBuilder();
                foreach (var module in modules)
                {
                    int speed = Convert.ToInt32(module["Speed"]);
                    string bankLabel = module["BankLabel"].ToString();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        sb.Append(String.Concat(bankLabel, ": ", speed, " MHz\n"));
                        LblRamSpeed.Text = sb.ToString();
                    });
                }
            }
        }

        private void GetStorage()
        {
            ulong totalSize = 0;
            ulong maxFreeSpace = 0;
            string maxDrive = "";

            ManagementObjectSearcher diskDriveSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            ManagementObjectCollection diskDrives = diskDriveSearcher.Get();

            foreach (ManagementObject diskDrive in diskDrives)
            {
                string deviceId = diskDrive["DeviceID"].ToString();
                ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{deviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
                ManagementObjectCollection partitions = partitionSearcher.Get();

                foreach (ManagementObject partition in partitions)
                {
                    string partitionDeviceId = partition["DeviceID"].ToString();
                    ManagementObjectSearcher logicalDiskSearcher = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionDeviceId}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");
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

            Application.Current.Dispatcher.Invoke(() =>
            {
                LblTotalStorage.Text = "Total Storage: " + totalSizeInGB.ToString() + " GB";
            });

            if (!string.IsNullOrEmpty(maxDrive))
            {
                var provider = new ManagementObject($"Win32_LogicalDisk.DeviceID='{maxDrive}'");
                ulong sizeInBytes = Convert.ToUInt64(provider["Size"]);
                ulong freeSpaceInBytes = Convert.ToUInt64(provider["FreeSpace"]);
                double freeSpaceInGB = Math.Round(Convert.ToDouble(freeSpaceInBytes) / (1024 * 1024 * 1024), 2);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LblFreeStorage.Text = $"{maxDrive} ({freeSpaceInGB} GB)";
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LblFreeStorage.Text = "No storage devices found.";
                });
            }
        }

        private void SampleTemp_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
