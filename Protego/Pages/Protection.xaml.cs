﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for Protection.xaml
    /// </summary>
    public partial class Protection : Page
    {
        public Protection()
        {
            InitializeComponent();
            MainContent.Content = new Sample();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Sample2();
        }
    }
}
