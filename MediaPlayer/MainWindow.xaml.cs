﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using WebCam_Capture;

namespace MediaPlayer
{
    public partial class MainWindow : Window
    {
        private double _savedVolume = 5;
        private List<string> _pathList = new List<string>();
        private int _listIndex = 0;
        private bool _isPause = false;
        private bool _isLoopAll = false;
        private bool _isLoopSingle = false;
        private bool _isPreview = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetPlayList(string dir, string[] str)
        {
            _pathList.Clear();
            for (int i = 0; i < str.Length; ++i)
                _pathList.Add(dir + str[i]);
            this.playList.ItemsSource = _pathList;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.ShowDialog();
            if (openFileDialog1.CheckFileExists)
                try
                {
                    string[] str = openFileDialog1.FileNames;
                    SetPlayList(openFileDialog1.InitialDirectory, str);
                    mediaElement.Source = new Uri(_pathList.First());
                    videoProgressBar.Value = 0;
                    mediaElement.Play();
                    /*if (mediaElement.HasAudio || mediaElement.HasAnimatedProperties || mediaElement.HasVideo)*/
                    videoProgressBar.Visibility = System.Windows.Visibility.Visible;
                    /*else
                        videoProgressBar.Visibility = System.Windows.Visibility.Hidden;*/
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
        }
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog openFolderDialog1 = new System.Windows.Forms.FolderBrowserDialog();//.OpenFileDialog();
            openFolderDialog1.ShowDialog();
            try
            {
                string[] str = Directory.GetFiles(openFolderDialog1.SelectedPath);
                SetPlayList("", str);
                mediaElement.Source = new Uri(str[0]);
                mediaElement.Play();
                videoProgressBar.Visibility = System.Windows.Visibility.Visible;
            }
            catch
            {
                MessageBox.Show("File could not be loaded!");
            }
        }

        private void OpenFromYouTube_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isPause)
            {
                mediaElement.Pause();
                _isPause = true;
            }
            else
            {
                mediaElement.Play();
                _isPause = false;
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            videoProgressBar.Value = 0;
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_listIndex > 0 && _listIndex <= _pathList.Count)
                try
                {
                    _listIndex--;
                    mediaElement.Stop();
                    mediaElement.Source = new Uri(_pathList[_listIndex]);
                    mediaElement.Play();
                    videoProgressBar.Value = 0;
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_listIndex >= 0 && _listIndex < _pathList.Count - 1)
                try
                {
                    _listIndex++;
                    mediaElement.Stop();
                    mediaElement.Source = new Uri(_pathList[_listIndex]);
                    mediaElement.Play();
                    videoProgressBar.Value = 0;
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
        }

        private void volumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (volumeBar.Value > 0)
            {
                _savedVolume = volumeBar.Value;
                volumeBar.Value = 0;
                //ImageSource iss = "/MediaPlayer;component/Images/speaker_mute.png";
                //iss.
                //ImageBrush ibbg = new ImageBrush();
                //volumeButton.Background = ibbg;
            }
            else
                volumeBar.Value = _savedVolume;
        }

        private void volumeBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = volumeBar.Value;
        }

        private void slowDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.SpeedRatio > 0.15)
            {
                mediaElement.SpeedRatio -= 0.05;
                speedLabel.Content = "x" + mediaElement.SpeedRatio;
            }
        }

        private void speedUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.SpeedRatio < 2.5)
            {
                mediaElement.SpeedRatio += 0.05;
                speedLabel.Content = "x" + mediaElement.SpeedRatio;
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid1.Width = Width - 22;
            grid1.Height = Height - 39;
        }


        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            if (_isLoopSingle)
                mediaElement.Source = new Uri(_pathList[_listIndex]);
            else if (_isLoopAll)
                try
                {
                    _listIndex++;
                    if (_listIndex >= _pathList.Count)
                        _listIndex = 0;
                    mediaElement.Source = new Uri(_pathList[_listIndex]);
                    mediaElement.Play();
                    videoProgressBar.Value = 0;
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
        }

        private void videoProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)videoProgressBar.Value);
                mediaElement.Position = ts;
            }
            catch
            {
            }
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            try 
            {
                this.videoProgressBar.Maximum = this.mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            }
            catch
            {
            }
        }

        private void loopAllButton_Click(object sender, RoutedEventArgs e)
        {
            this._isLoopAll = this._isLoopAll ? false : true;
        }

        private void LoopSingleButton_Click(object sender, RoutedEventArgs e)
        {

            this._isLoopSingle = this._isLoopSingle ? false : true;
        }

        private void fullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == System.Windows.WindowState.Maximized ? System.Windows.WindowState.Normal : System.Windows.WindowState.Maximized;
        }

        private void showPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            _isPreview = _isPreview ? false : true;
            mediaElement.Visibility = _isPreview ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            mediaElementBackground.Visibility = _isPreview ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            grid2.Visibility = _isPreview ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void cameraButton_Click(object sender, RoutedEventArgs e)
        {
            CaptureWindow capturWin = new CaptureWindow();
            capturWin.Show();
        }

        private void MediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}