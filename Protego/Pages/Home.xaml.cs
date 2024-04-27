using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Protego.UserControls;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private readonly PerformanceCounter perfRAM = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public Home()
        {
            InitializeComponent();
            
        }

       
    }
}
