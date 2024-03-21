﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Threading;

namespace Protego.Pages
{
    /// <summary>
    /// Interaction logic for Protection.xaml
    /// </summary>
    public partial class Protection : Page
    {
        
        public ObservableCollection<string> FileList { get; set; }


        public Protection()
        {
            InitializeComponent();
            FileList = new ObservableCollection<string>();
            DataContext = this;
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            FileList.Clear(); // Clear the previous list
            string directoryPath = @"d:";

            if (Directory.Exists(directoryPath))
            {
                ScanDirectory(directoryPath);
            }
            else
            {
                MessageBox.Show("Directory does not exist!");
            }
        }

        private void ScanDirectory(string directory)
        {
            try
            {
                // Get all files in the directory and add them to the list
                string[] files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    FileList.Add(file);
                }

                // Recursively scan subdirectories
                string[] subDirectories = Directory.GetDirectories(directory);
                foreach (string subDir in subDirectories)
                {
                    ScanDirectory(subDir);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning directory: {ex.Message}");
            }
        }



    }
}