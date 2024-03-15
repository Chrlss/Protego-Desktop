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

            GetOSInfo();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            GetTime();
        }
        private void GetTime()
        {
            DateTime date;
            date = DateTime.Now;
            LblClock.Text = date.ToLongDateString() + "   " + date.ToLongTimeString();
        }
        private void GetOSInfo()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();

            foreach (var provider in providers)
            {
                string procStatus = provider["Status"].ToString();
                int procFamily = Convert.ToInt16(provider["Family"]);

                LblStatus.Text = "Processor Status" + " " + procStatus.ToString();

                if (procFamily == 107)
                {
                    LblSku.Text = "AMD Ryzen 5 5600G";
                }

                if (procFamily == 11)
                {
                    LblSku.Text = "Pentium(R) brand ";
                }

                if (procFamily == 12)
                {
                    LblSku.Text = "Pentium(R) Pro ";
                }

                if (procFamily == 13)
                {
                    LblSku.Text = "Pentium(R) II ";

                }

                if (procFamily == 14)
                {
                    LblSku.Text = "Pentium(R) processor with MMX(TM) technology";
                }

                if (procFamily == 14)
                {
                    LblSku.Text = "Pentium(R) processor with MMX(TM) technology";
                }

                if (procFamily == 15)
                {
                    LblSku.Text = "Celeron(TM)";
                }

                if (procFamily == 16)
                {
                    LblSku.Text = "Pentium(R) II Xeon(TM) ";
                }
                if (procFamily == 17)
                {
                    LblSku.Text = "Pentium(R) III ";
                }
                if (procFamily == 20)
                {
                    LblSku.Text = "Intel(R) Celeron(R) M processor) ";
                }
                if (procFamily == 21)
                {
                    LblSku.Text = "Intel(R) Pentium(R) 4 HT processor ";
                }
                if (procFamily == 28)
                {
                    LblSku.Text = "AMD Athlon(TM) Processor Family ";
                }
                if (procFamily == 29)
                {
                    LblSku.Text = "AMD(R) Duron(TM) Processor ";
                }
                if (procFamily == 30)
                {
                    LblSku.Text = "AMD29000 Family  ";
                }
                if (procFamily == 40)
                {
                    LblSku.Text = "Intel(R) Core(TM) Duo processor  ";
                }
                if (procFamily == 41)
                {
                    LblSku.Text = "Intel(R) Core(TM) Duo mobile processor ";
                }
                if (procFamily == 42)
                {
                    LblSku.Text = "Intel(R) Core(TM) Solo mobile processor ";
                }
                if (procFamily == 43)
                {
                    LblSku.Text = "Intel(R) Atom(TM) processor ";
                }
                if (procFamily == 58)
                {
                    LblSku.Text = "AMD Athlon(TM) II Dual-Core Mobile M Processor Family ";
                }
                if (procFamily == 131)
                {
                    LblSku.Text = "AMD Athlon(TM) 64 Processor Family ";
                }
                if (procFamily == 132)
                {
                    LblSku.Text = "AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 133)
                {
                    LblSku.Text = "AMD Sempron(TM) Processor Family ";
                }
                if (procFamily == 134)
                {
                    LblSku.Text = "AMD Sempron(TM) Processor Family ";
                }
                if (procFamily == 135)
                {
                    LblSku.Text = "Dual-Core AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 136)
                {
                    LblSku.Text = "AMD Athlon(TM) 64 X2 Dual-Core Processor Family ";
                }
                if (procFamily == 137)
                {
                    LblSku.Text = "AMD Turion(TM) 64 X2 Mobile Technology ";
                }
                if (procFamily == 138)
                {
                    LblSku.Text = "Quad-Core AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 139)
                {
                    LblSku.Text = "Third-Generation AMD Opteron(TM) Processor Family ";
                }
                if (procFamily == 140)
                {
                    LblSku.Text = "AMD Phenom(TM) FX Quad-Core Processor Family ";
                }
                if (procFamily == 141)
                {
                    LblSku.Text = "AMD Phenom(TM) X4 Quad-Core Processor Family ";
                }
                if (procFamily == 142)
                {
                    LblSku.Text = "AMD Phenom(TM) X2 Dual-Core Processor Family  ";
                }
                if (procFamily == 143)
                {
                    LblSku.Text = "AMD Athlon(TM) X2 Dual-Core Processor Family ";
                }
                if (procFamily == 198)
                {
                    LblSku.Text = "Intel(R) Core(TM) i7 processor ";
                }
                if (procFamily == 199)
                {
                    LblSku.Text = "Dual-Core Intel(R) Celeron(R) Processor ";
                }
                if (procFamily == 205)
                {
                    LblSku.Text = "Intel(R) Core(TM) i5 processor ";
                }
                if (procFamily == 206)
                {
                    LblSku.Text = " Intel(R) Core(TM) i3 processor";
                }
                if (procFamily == 207)
                {
                    LblSku.Text = "Intel(R) Core(TM) i9 processor  ";
                }
                if (procFamily == 234)
                {
                    LblSku.Text = " AMD Athlon(TM) Dual-Core Processor Family ";
                }
                if (procFamily == 235)
                {
                    LblSku.Text = "AMD Sempron(TM) SI Processor Family ";
                }
                if (procFamily == 236)
                {
                    LblSku.Text = "AMD Phenom(TM) II Processor Family ";
                }
                if (procFamily == 237)
                {
                    LblSku.Text = "AMD Athlon(TM) II Processor Family ";
                }
               
                

            }
        }
        
    }
}
