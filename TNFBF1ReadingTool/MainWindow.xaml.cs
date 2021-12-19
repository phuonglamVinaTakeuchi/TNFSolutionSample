﻿using Microsoft.Win32;
using System.Windows;
using TNFDataModel.TNFFile;

namespace TNFBF1ReadingTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "bf1 files (*.bf1)|*.bf1";
            if (openFileDialog.ShowDialog() == true)
            {
                var bf1ReadingFile = new TNFReading(openFileDialog.FileName);
                var bf1Data = new BF1FileDataModel(openFileDialog.FileName, bf1ReadingFile.Contents, "BF1 Data Blog");

            }
        }
    }
}
