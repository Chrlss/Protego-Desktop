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
using System.Windows.Media.Animation;

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


            GetOSInfo();
            GetRamInfo();
            GetStorage();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            GetOSInfo();
        }
        private void GetOSInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();
            #region //procie
            foreach (var provider in providers)
            {
                int clock = Convert.ToInt32(provider["MaxClockSpeed"]);
                int procFamily = Convert.ToInt16(provider["Family"]);

                LblClock.Text = $"{clock} MHz";
                //LblClock.Text = clock.ToString();

                if (procFamily == 107)
                {
                    LblProcie.Text = "AMD Ryzen 5 5600G";
                }

                if (procFamily == 11)
                {
                    LblProcie.Text = "Pentium(R) brand ";
                }

                if (procFamily == 12)
                {
                    LblProcie.Text = "Pentium(R) Pro ";
                }

                if (procFamily == 13)
                {
                    LblProcie.Text = "Pentium(R) II ";

                }

                if (procFamily == 14)
                {
                    LblProcie.Text = "Pentium(R) processor with MMX(TM) technology";
                }

                if (procFamily == 14)
                {
                    LblProcie.Text = "Pentium(R) processor with MMX(TM) technology";
                }

                if (procFamily == 15)
                {
                    LblProcie.Text = "Celeron(TM)";
                }

                if (procFamily == 16)
                {
                    LblProcie.Text = "Pentium(R) II Xeon(TM) ";
                }
                if (procFamily == 17)
                {
                    LblProcie.Text = "Pentium(R) III ";
                }
                if (procFamily == 20)
                {
                    LblProcie.Text = "Intel(R) Celeron(R) M processor) ";
                }
                if (procFamily == 21)
                {
                    LblProcie.Text = "Intel(R) Pentium(R) 4 HT processor ";
                }
                if (procFamily == 28)
                {
                    LblProcie.Text = "AMD Athlon(TM) Processor Family ";
                }
                if (procFamily == 29)
                {
                    LblProcie.Text = "AMD(R) Duron(TM) Processor ";
                }
                if (procFamily == 30)
                {
                    LblProcie.Text = "AMD29000 Family  ";
                }
                if (procFamily == 40)
                {
                    LblProcie.Text = "Intel(R) Core(TM) Duo processor  ";
                }
                if (procFamily == 41)
                {
                    LblProcie.Text = "Intel(R) Core(TM) Duo mobile processor ";
                }
                if (procFamily == 42)
                {
                    LblProcie.Text = "Intel(R) Core(TM) Solo mobile processor ";
                }
                if (procFamily == 43)
                {
                    LblProcie.Text = "Intel(R) Atom(TM) processor ";
                }
                if (procFamily == 58)
                {
                    LblProcie.Text = "AMD Athlon(TM) II Dual-Core Mobile M Processor Family ";
                }
                if (procFamily == 131)
                {
                    LblProcie.Text = "AMD Athlon(TM) 64 Processor Family ";
                }
                if (procFamily == 132)
                {
                    LblProcie.Text = "AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 133)
                {
                    LblProcie.Text = "AMD Sempron(TM) Processor Family ";
                }
                if (procFamily == 134)
                {
                    LblProcie.Text = "AMD Sempron(TM) Processor Family ";
                }
                if (procFamily == 135)
                {
                    LblProcie.Text = "Dual-Core AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 136)
                {
                    LblProcie.Text = "AMD Athlon(TM) 64 X2 Dual-Core Processor Family ";
                }
                if (procFamily == 137)
                {
                    LblProcie.Text = "AMD Turion(TM) 64 X2 Mobile Technology ";
                }
                if (procFamily == 138)
                {
                    LblProcie.Text = "Quad-Core AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 139)
                {
                    LblProcie.Text = "Third-Generation AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 140)
                {
                    LblProcie.Text = "AMD Phenom(TM) FX Quad-Core Processor Family ";
                }
                if (procFamily == 141)
                {
                    LblProcie.Text = "AMD Phenom(TM) X4 Quad-Core Processor Family ";
                }
                if (procFamily == 142)
                {
                    LblProcie.Text = "AMD Phenom(TM) X2 Dual-Core Processor Family  ";
                }
                if (procFamily == 143)
                {
                    LblProcie.Text = "AMD Athlon(TM) X2 Dual-Core Processor Family ";
                }
                if (procFamily == 198)
                {
                    LblProcie.Text = "Intel(R) Core(TM) i7 processor ";
                }
                if (procFamily == 199)
                {
                    LblProcie.Text = "Dual-Core Intel(R) Celeron(R) Processor ";
                }
                if (procFamily == 205)
                {
                    LblProcie.Text = "Intel(R) Core(TM) i5 processor ";
                }
                if (procFamily == 206)
                {
                    LblProcie.Text = " Intel(R) Core(TM) i3 processor";
                }
                if (procFamily == 207)
                {
                    LblProcie.Text = "Intel(R) Core(TM) i9 processor  ";
                }
                if (procFamily == 234)
                {
                    LblProcie.Text = " AMD Athlon(TM) Dual-Core Processor Family ";
                }
                if (procFamily == 235)
                {
                    LblProcie.Text = "AMD Sempron(TM) SI Processor Family ";
                }
                if (procFamily == 236)
                {
                    LblProcie.Text = "AMD Phenom(TM) II Processor Family ";
                }
                if (procFamily == 237)
                {
                    LblProcie.Text = "AMD Athlon(TM) II Processor Family ";
                }
            }
            #endregion
        }
        private void GetRamInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_ComputerSystem");
            var system = wmi.GetInstances().Cast<System.Management.ManagementObject>().FirstOrDefault();
            if (system != null)
            {
                long totalPhysicalMemory = Convert.ToInt64(system["TotalPhysicalMemory"]);
                LblTotaRam.Text = $"{totalPhysicalMemory / (1024 * 1024)} MB";
            }
            wmi = new System.Management.ManagementClass("Win32_PhysicalMemory");
            var modules = wmi.GetInstances();
            foreach (var module in modules)
            {
                int speed = Convert.ToInt32(module["Speed"]);
                string bankLabel = module["BankLabel"].ToString();
                LblRamSpeed.Text += $"{bankLabel}: {speed} MHz\n";
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
            LblTotalStorage.Text = "Total Storage: " + totalSizeInGB.ToString() + " GB";

            if (maxDrive != "")
            {
                var provider = new ManagementObject("Win32_LogicalDisk.DeviceID='" + maxDrive + "'");
                ulong sizeInBytes = Convert.ToUInt64(provider["Size"]);
                ulong freeSpaceInBytes = Convert.ToUInt64(provider["FreeSpace"]);
                double freeSpaceInGB = Math.Round(Convert.ToDouble(freeSpaceInBytes) / (1024 * 1024 * 1024), 2);

                LblFreeStorage.Text = maxDrive + " (" + freeSpaceInGB.ToString() + " GB)";
            }
            else
            {
                LblFreeStorage.Text = "No storage devices found.";
            }
        }
    }
}