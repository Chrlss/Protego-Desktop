using Protego.Pages;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Converters;




namespace Protego
{
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point initialMousePosition;

        

        public MainWindow()
        {
            InitializeComponent();

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

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LogInWindow logIn = new LogInWindow();
            logIn.Show();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}