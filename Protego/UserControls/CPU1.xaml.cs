using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Protego.UserControls
{
    /// <summary>
    /// Interaction logic for CPU1.xaml
    /// </summary>
    public partial class CPU1 : UserControl
    {
        private PerformanceCounter cpuCounter;
        public CPU1()
        {
            InitializeComponent();
            // Create a PerformanceCounter to get the processor's clock speed
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            // Create a Binding to bind the ProgressBar's Value property to the clock speed
            Binding binding = new Binding();
            binding.Source = cpuCounter;
            binding.Path = new PropertyPath("NextValue");
            binding.Converter = new ClockSpeedConverter();
            binding.Mode = BindingMode.OneWay;

            // Set the binding to the ProgressBar
            cpuClockProgressBar.SetBinding(ProgressBar.ValueProperty, binding);

            // Create a DispatcherTimer to update the clock speed every second
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the clock speed
            cpuCounter.NextValue();
        }

        public class ClockSpeedConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                int clockSpeed = (int)value;
                int maxValue = 100; // Set the maximum value of the ProgressBar to 100
                int progressBarValue = (int)((double)clockSpeed / 100 * maxValue);
                return progressBarValue;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }



    }
}
