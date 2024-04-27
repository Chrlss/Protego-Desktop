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

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Security.xaml
    /// </summary>
    public partial class Security : Page
    {
        public Security()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string targetUrl = "https://github.com/SaibaDev/Protego-Web-extension";
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", targetUrl);
            }
            catch (Exception ex)
            {
                // Handle exception, for example, display an error message to the user
                MessageBox.Show("Failed to open link: " + ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string targetUrl = "https://saibadev.github.io/Protego-website-deploy/protegoNAS.html";
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", targetUrl);
            }
            catch (Exception ex)
            {
                // Handle exception, for example, display an error message to the user
                MessageBox.Show("Failed to open link: " + ex.Message);
            }
        }
    }
}
