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
using System.Windows.Shapes;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        private bool isDragging = false;
        private Point initialMousePosition;
        public LogIn()
        {
            InitializeComponent();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else WindowState = WindowState.Maximized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            initialMousePosition = e.GetPosition(this);
        }

        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Vector offset = e.GetPosition(this) - initialMousePosition;
                this.Left += offset.X;
                this.Top += offset.Y;
            }
        }

        private void StackPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }


    }
}
