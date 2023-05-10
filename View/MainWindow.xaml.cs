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
            try
            {
                Converter.sampleRate = int.Parse(sampleRate.Text);
                Converter.bitDepth = int.Parse(bitDepth.Text);
                Converter.channels = int.Parse(channels.Text);
            }
            catch(FormatException ex) {
                // do nothing lol
            }
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

        private void PlaySound(object sender, RoutedEventArgs e)
        {
            try
            {
                Converter.sampleRate = int.Parse(sampleRate.Text);
                Converter.bitDepth = int.Parse(bitDepth.Text);
                Converter.channels = int.Parse(channels.Text);
            }
            catch (FormatException ex)
            {
                // do nthing lol
            }
            Converter.Play(this.FilePath.Text);
        }

        private void StreamAudio(object sender, RoutedEventArgs e)
        {
            Converter.StreamAudio(this.IP.Text);
        }

        private void ReceiveAudio(object sender, RoutedEventArgs e)
        {
            Converter.ReceiveAudio(this.IP_Receiver.Text);
        }
    }
}
