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
using MediaPlayer.ViewModel;
using MediaPlayer.Model;
using System.Collections.ObjectModel;

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
        private bool _isShuffle = false;
        private int _timeDurationSize = 2;
        private string[] _allowedExt = { ".mp3", ".mp4", ".asf", ".3gp", ".3g2", ".asx", ".avi", ".jpg", ".jpeg", ".gif", ".bmp", ".png" };

        private WebCam _webcam;
        private CurrentPlaylist _playList;
        private CurrentPlaylist _savePlayList;

        private System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            initTimer();
            _webcam = new WebCam();
            _playList = new CurrentPlaylist();
            _webcam.InitializeWebCam(ref captureImage);
        }

        ~MainWindow()
        {
            _timer.Stop();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid1.Width = ActualWidth - 16;
            grid1.Height = ActualHeight - 40;
            Tabulations.Width = ActualWidth - 16;
            Tabulations.Height = ActualHeight - 38.3;
        }

        // UTILS METHODS

        private void initTimer()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(synchronizeProgressBar);
            _timer.Start();
        }

        private string getName(string path)
        {
            int lastSlash = path.LastIndexOf('/'), lastPoint = path.LastIndexOf('.');
            if (lastPoint != -1 && lastPoint < path.Length)
                path = path.Substring(0, lastPoint);
            if (lastSlash != -1 && lastSlash + 1 < path.Length)
                path = path.Substring(lastSlash + 1);
            return path;
        }

        private string formatTime(System.TimeSpan time)
        {
            string res = "";

            if (time.Hours > 0 || this._timeDurationSize == 3)
                res += time.Hours + ":";
            if ((time.Hours > 0 || this._timeDurationSize == 3) && time.Minutes < 10)
                res += "0" + time.Minutes + ":";
            else
                res += time.Minutes + ":";
            if (time.Seconds < 10)
                res += "0" + time.Seconds;
            else
                res += time.Seconds;
            return res;
        }

        private bool checkUrl(string url)
        {
            string pat = @"http[s]?://www\.youtube\.com/watch\?v=(.*)$";

            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            return r.Match(url).Success;
        }

        private string formatUrl(string url)
        {
            return url.Replace("watch?v=", "embed/");
        }

        private ImageBrush loadImage(string imagePath)
        {
            var brush = new ImageBrush();
            try
            {
                Uri resourceUri = new Uri(imagePath, UriKind.Relative);
                System.Windows.Resources.StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                brush.ImageSource = temp;
            }
            catch
            {
                MessageBox.Show("File '" + imagePath + "' don't exist");
            }
            return brush;
        }

        // MEDIA PLAYER

        private void goNextMedia()
        {
            if (_listIndex >= 0 && _listIndex < this._playList.Count())
            {
                try
                {
                    if (_listIndex >= this._playList.Count() - 1 && !_isLoopAll)
                        return;
                    _listIndex++;
                    if (_listIndex >= this._playList.Count())
                        _listIndex = 0;
                    mediaElement.Source = new Uri(_playList.getMediaPath(_listIndex));
                    videoProgressBar.Value = 0;
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
            }
        }

        private void goPreviousMedia()
        {
            if (_listIndex >= 0 && _listIndex < this._playList.Count())
            {
                try
                {
                    if (_listIndex <= 0 && !_isLoopAll)
                        return;
                    _listIndex--;
                    if (_listIndex < 0)
                        _listIndex = this._playList.Count() - 1;
                    mediaElement.Source = new Uri(_playList.getMediaPath(_listIndex));
                    videoProgressBar.Value = 0;
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
            }
        }

        private void playMedia()
        {
            this._isPause = false;
            mediaElement.Play();
            playButton.Background = this.loadImage("Images/PauseCommu.png");
        }

        private void pauseMedia()
        {
            this._isPause = true;
            playButton.Background = this.loadImage("Images/PlayCommu.png");
            mediaElement.Pause();
        }

        private void stopMedia()
        {
            this._isPause = false;
            playButton.Background = this.loadImage("Images/PlayCommu.png");
            mediaElement.Stop();
            videoProgressBar.Value = 0;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Multiselect = true;
            System.Windows.Forms.DialogResult res = openFileDialog1.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK && openFileDialog1.CheckFileExists)
            {
                try
                {
                    string[] str = openFileDialog1.FileNames;
                    this._playList.addFiles(openFileDialog1.InitialDirectory, openFileDialog1.FileNames);
                    this.playList.ItemsSource = this._playList.getPlayList();

                    if (mediaElement.Source == null)
                    {
                        mediaElement.Source = new Uri(this._playList.getMediaPath(0));
                        this.playMedia();
                    }
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog openFolderDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            if (openFolderDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    this._playList.addFolder(this.getFilesWithAllowedExt(Directory.GetFiles(openFolderDialog1.SelectedPath)));
                    this.playList.ItemsSource = this._playList.getPlayList();
     				if (mediaElement.Source == null)
                    {
                        mediaElement.Source = new Uri(this._playList.getMediaPath(0));
                        this.playMedia();
                    }                }
                catch
                {
                    MessageBox.Show("File could not be loaded!");
                }
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isPause)
                this.pauseMedia();
            else
                this.playMedia();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            this.stopMedia();
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            this.goPreviousMedia();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            this.goNextMedia();
        }

        private void volumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (volumeBar.Value > 0)
            {
                _savedVolume = volumeBar.Value;
                volumeBar.Value = 0;
                volumeButton.Background = this.loadImage("Images/MuteCommu.png");
            }
            else
            {
                volumeBar.Value = _savedVolume;
                volumeButton.Background = this.loadImage("Images/volumeCommu.png");
            }
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

        private void refreshLoopsButons()
        {
            if (this._isLoopAll)
                loopAllButton.Background = this.loadImage("Images/loopActivCommu.png");
            else
                loopAllButton.Background = this.loadImage("Images/LoopAllCommu.png");
            if (this._isLoopSingle)
                LoopSingleButton.Background = this.loadImage("Images/loopOneActiveCommu.png");
            else
                LoopSingleButton.Background = this.loadImage("Images/LoopOneCommu.png");
        }

        private void loopAllButton_Click(object sender, RoutedEventArgs e)
        {
            this._isLoopAll = this._isLoopAll ? false : true;
            if (this._isLoopAll)
                this._isLoopSingle = false;
            this.refreshLoopsButons();
        }

        private void LoopSingleButton_Click(object sender, RoutedEventArgs e)
        {
            this._isLoopSingle = this._isLoopSingle ? false : true;
            if (this._isLoopSingle)
                this._isLoopAll = false;
            this.refreshLoopsButons();
        }

        private void showPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            TabPlaylists.Focus();
        }



        private void Random_Click(object sender, RoutedEventArgs e)
        {
            this._isShuffle = this._isShuffle ? false : true;
            if (this._isShuffle)
            {
                //this._savePlaylist = new CurrentPlaylist(this._playList);
                this._playList.shuffle();
                Random.Background = this.loadImage("Images/shuffleActiveCommu.png");
            }
            else
            {
                //this._playList =  new CurrentPlaylist(this._savePlaylist);
                Random.Background = this.loadImage("Images/ShuffleCommu.png");
            }
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.stopMedia();
            if (this._isLoopSingle)
                this.playMedia();
            else
                this.goNextMedia();
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                System.TimeSpan total = this.mediaElement.NaturalDuration.TimeSpan;
                this._timeDurationSize = 2;
                this.videoProgressBar.Maximum = total.TotalSeconds;
                this.totalTimeLabel.Content = this.formatTime(total);
                this.currentTimeLabel.Content = this.formatTime(new System.TimeSpan(0, 0, 0));
                this._timeDurationSize = total.Hours > 0 ? 3 : 2;
                mediaTitle.Content = this._playList.getMediaTitle(this._listIndex);//getName(mediaElement.Source.ToString());
                //GridMusicInfos.Visibility = System.Windows.Visibility.Hidden;
                if (mediaElement.HasAudio || mediaElement.HasVideo)
                    GridProgressBar.Visibility = System.Windows.Visibility.Visible;
                else
                    GridProgressBar.Visibility = System.Windows.Visibility.Hidden;
            }
            catch
            {
            }
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
        }

        void synchronizeProgressBar(object sender, EventArgs e)
        {
            if ((mediaElement.HasVideo || mediaElement.HasAudio) && !_isPause)
            {
                double pos = mediaElement.Position.TotalSeconds;
                if (pos > videoProgressBar.Value)
                    videoProgressBar.Value = pos;
                this.currentTimeLabel.Content = this.formatTime(new System.TimeSpan(0, 0, (int)mediaElement.Position.TotalSeconds));
            }
        }

        private void videoProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                double prog = videoProgressBar.Value;
                if (Math.Abs(prog - mediaElement.Position.TotalSeconds) > 2)
                {
                    TimeSpan ts = new TimeSpan(0, 0, 0, (int)prog);
                    mediaElement.Position = ts;
                }
            }
            catch
            {
            }
        }


        //PLAYLISTS


        private void SetPlayList(string dir, string[] str)
        {
            _pathList.Clear();
            for (int i = 0; i < str.Length; ++i)
                _pathList.Add(dir + str[i]);
            //this.playList.ItemsSource = _pathList;
            this.playList.ItemsSource = this._playList.getPlayList();
        }

        //Youtube


        private void YoutubeButton_Click(object sender, RoutedEventArgs e)
        {
            string url = YoutubeLink.Text;

            if (checkUrl(url))
            {
                url = this.formatUrl(url);
                YoutubeEmbededPlayer.Navigate(this.formatUrl(url));
            }
            else
            {
                MessageBox.Show("Le lien n'est pas valide !");
            }
        }

        private void YoutubeLink_GotFocus(object sender, RoutedEventArgs e)
        {
            if (YoutubeLink.Text == "Insert Youtube URL here")
                YoutubeLink.Text = "";
            YoutubeLink.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void YoutubeLink_LostFocus(object sender, RoutedEventArgs e)
        {
            if (YoutubeLink.Text == "")
                YoutubeLink.Text = "Insert Youtube URL here";
            YoutubeLink.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (YoutubeLink.Text == "Insert Youtube URL here")
                YoutubeLink.Foreground = new SolidColorBrush(Colors.Gray);
        }

        // CAMERA CAPTURE


        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.SaveImageCapture((BitmapSource)captureImage.Source);
        }

        private void CamCaptureTab_GotFocus(object sender, RoutedEventArgs e)
        {
            _webcam.Start();
        }

        private void CamCaptureTab_LostFocus(object sender, RoutedEventArgs e)
        {
            _webcam.Stop();
        }

        private void mediaElementBackground_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            this._playList.addFolder(files);
            this.playList.ItemsSource = this._playList.getPlayList();
            if (!this._isPause)
            {
                mediaElement.Source = new Uri(this._playList.getMediaPath(this._listIndex));
                this.playMedia();
            }
        }
    }
}
