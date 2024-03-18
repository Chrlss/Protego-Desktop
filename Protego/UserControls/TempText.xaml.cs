using LibreHardwareMonitor.Hardware;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Protego.UserControls
{
    /// <summary>
    /// Interaction logic for TempText.xaml
    /// </summary>
    public partial class TempText : UserControl
    {
        private Computer computer;
        private DispatcherTimer dispatcherTimer;
        public TempText()
        {
            InitializeComponent();

            computer = new Computer { IsCpuEnabled = true };
            computer.Open();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1); // Update every second
            dispatcherTimer.Start();

           Task.Run(() => InitializeCpuTemperatureMonitoring());
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => InitializeCpuTemperatureMonitoring());
        }

        private void InitializeCpuTemperatureMonitoring()
        {
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    hardwareItem.Update();
                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core"))
                        {
                            UpdateCpuTemperature((float)sensor.Value);
                            return;
                        }
                    }
                }
            }
        }
        private void UpdateCpuTemperature(float temperature)
        {
            Dispatcher.Invoke(() =>
            {
                TempCel.Text = $"{temperature}°C";
                

            });
        }
    }
}
