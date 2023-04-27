using Logic;
using Microsoft.Win32;
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

namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SoundConverter Converter;
        public MainWindow()
        {
            InitializeComponent();
            Converter = new SoundConverter();
           
        }

        private void StartRecording(object sender, RoutedEventArgs e)
        {
            Converter.SetSavePath(this.FilePath.Text);
            Converter.StartRecording();
        }

        private void StopRecording(object sender, RoutedEventArgs e)
        {
            Converter.StopRecording();
        }
        private void GetPath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            string filePath;
            if (open.ShowDialog() == true)
            {
                this.FilePath.Text = open.FileName;
                // Save the file
            }
            else
            {
                return;
            }
        }
    }
}
