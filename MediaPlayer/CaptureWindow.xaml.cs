using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private WebCam webcam;

        public CaptureWindow()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.SaveImageCapture((BitmapSource)captureImage.Source);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref captureImage);
            webcam.Start();
            webcam.Continue();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            webcam.Stop();
        }
    }
}
