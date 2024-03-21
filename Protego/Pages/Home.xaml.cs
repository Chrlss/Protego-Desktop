using Protego.UserControls;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using LibreHardwareMonitor.Hardware;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        PerformanceCounter perfRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        
        public Home()
        {
            InitializeComponent();
            
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();



            //GetOSInfo();
            Task.Run(() => ProcessorFamily());
            
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            RAM.Value = (int)perfRAM.NextValue();
            RAMpercent.Text = RAM.Value.ToString() + "%";
            Task.Run(() => ProcessorFamily());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
        }

        private void ProcessorFamily()
        {
            System.Management.ManagementClass wmi = new System.Management.ManagementClass("Win32_Processor");
            var providers = wmi.GetInstances();
            StringBuilder sbFamily = new StringBuilder();

            foreach (var provider in providers)
            {
                int ProcFamily = Convert.ToInt16(provider["Family"]);
                
                if (ProcFamily == 198)
                {
                    sbFamily.Append("Intel(R) Core(TM) i7");
                }
                if (ProcFamily == 107)
                {
                    sbFamily.Append("AMD Ryzen 5 5600G");
                }
                if (ProcFamily == 11)
                {
                    sbFamily.Append("Pentium(R) brand");
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                LblProcFamily.Text = sbFamily.ToString();
            });

        }

        
    }
}
