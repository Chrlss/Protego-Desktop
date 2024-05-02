using Protego.Pages;
using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Protego.Navigation;


namespace Protego
{
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point initialMousePosition;
        
        private ManagementEventWatcher _deviceWatcher;



        public MainWindow()
        {
            InitializeComponent();            
            Loaded += (_, _) => NavMenu.Navigate(typeof(Home));           
            InitializeDeviceWatcher();

            ContentFrame.Navigate(typeof(Home));
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StatusBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void InitializeDeviceWatcher()
        {
            
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2"); 

            
            _deviceWatcher = new ManagementEventWatcher(query);
            _deviceWatcher.EventArrived += DeviceInsertedHandler;
            _deviceWatcher.Start();
        }

        private void DeviceInsertedHandler(object sender, EventArrivedEventArgs e)
        {
            
            Dispatcher.Invoke(() => NavMenu.Navigate(typeof(Protection))); 
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_deviceWatcher != null)
            {
                _deviceWatcher.Stop();
                _deviceWatcher.Dispose();
            }
        }
    }
}
