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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private bool isDragging = false;
        private Point initialMousePosition;
        private BlurEffect blurEffect;
        public LogInWindow()
        {
            InitializeComponent();
            blurEffect = new BlurEffect();
            blurEffect.KernelType = KernelType.Gaussian;
            blurEffect.Radius = 5;

            var border = this.FindName("MyBorder") as Border;
            if (border != null)
            {
                border.Effect = blurEffect;
            }

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
