using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PerformanceCounter cpuCounter;

        public MainWindow()
        {
            InitializeComponent();

            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";

            // Update rectangle width on load and every second
            UpdateRectangleWidth();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) => UpdateRectangleWidth();
            timer.Start();
        }

        private void UpdateRectangleWidth()
        {
            try
            {
                double cpuUsage = cpuCounter.NextValue();
                cpuRectangle.Width = Math.Min(cpuUsage, 100) * ActualWidth / 100; // Clamp to 100%
                cpuPercentageLabel.Content = cpuUsage.ToString("F1") + "%"; // Display with one decimal place
            }
            catch (Exception ex)
            {
                // Handle potential exceptions gracefully (e.g., log error, display error message)
                Console.WriteLine("Error updating CPU usage: " + ex.Message);
            }
        }
    }
}