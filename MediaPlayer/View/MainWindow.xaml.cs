using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using MediaPlayer.ViewModel;
using MediaPlayer.Model;
using System.Collections.ObjectModel;
using MediaPlayer.View;

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
        private bool _fullScreen = false;
        private int _timeDurationSize = 2;

        private WebCam _webcam;
        private CurrentPlaylist _playList;
        private CurrentUsers _users;
        private PlaylistManager _playlistManager;

        private System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            initTimer();
            _webcam = new WebCam();
            _playList = new CurrentPlaylist();
            _users = new CurrentUsers();
            _webcam.InitializeWebCam(ref captureImage);
            this.hideAudioElements();
            this._playlistManager = new PlaylistManager();
        }

        ~MainWindow()
        {
            _timer.Stop();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid1.Width = ActualWidth - 16;
            grid1.Height = ActualHeight - 40;
        }

        // UTILS METHODS

        private void initTimer()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(synchronizeProgressBar);
            _timer.Start();
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
                MessageBox.Show("File '" + imagePath + "' don't exist", "Path Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
            return brush;
        }

        // MEDIA PLAYER

        private void loadAndPlayMedia()
        {
            this.hideAudioElements();
            mediaElement.Source = new Uri(_playList.getMediaPath(_listIndex));
            videoProgressBar.Value = 0;
        }
        
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
                    this.loadAndPlayMedia();
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
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
                    this.loadAndPlayMedia();
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                }
            }
        }

        private void playMedia()
        {
            this._isPause = false;
            hideAudioElements();
            mediaElement.Play();
            playButton.Background = this.loadImage("../Images/PauseCommu.png");
        }

        private void pauseMedia()
        {
            this._isPause = true;
            playButton.Background = this.loadImage("../Images/PlayCommu.png");
            mediaElement.Pause();
        }

        private void stopMedia()
        {
            this._isPause = true;
            playButton.Background = this.loadImage("../Images/PlayCommu.png");
            mediaElement.Stop();
            currentTimeLabel.Content = "0:00";
            videoProgressBar.Value = 0;
        }

        private void displayAudioElements(IMedia currentMedia)
        {
            musicTitle.Content = "Title:\t" + currentMedia.Title;
            musicSinger.Content = "Artist:\t" + currentMedia.Artist;
            musicAlbum.Content = "Album:\t" + currentMedia.Album;
            musicGenre.Content = "Genre:\t" + currentMedia.Genre;
            musicYear.Content = "Year:\t" + currentMedia.Year;
            GridMusicInfos.Visibility = System.Windows.Visibility.Visible;
        }

        private void hideAudioElements()
        {
            GridMusicInfos.Visibility = System.Windows.Visibility.Hidden;
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
                    this._playList.addFiles(openFileDialog1.InitialDirectory, openFileDialog1.FileNames);
                    this.playList.ItemsSource = this._playList.getPlayList();
                    if (mediaElement.Source == null && this._playList.getPlayList().Count() > 0)
                    {
                        mediaElement.Source = new Uri(this._playList.getMediaPath(0));
                        this.playMedia();
                    }
                    this._isShuffle = false;
                    Random.Background = this.loadImage("../Images/ShuffleCommu.png");
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
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
                    this._playList.addFolder(Directory.GetFiles(openFolderDialog1.SelectedPath));
                    this.playList.ItemsSource = this._playList.getPlayList();
                    if (mediaElement.Source == null && this._playList.getPlayList().Count() > 0)
                    {
                        mediaElement.Source = new Uri(this._playList.getMediaPath(0));
                        this.playMedia();
                    }
                    this._isShuffle = false;
                    Random.Background = this.loadImage("../Images/ShuffleCommu.png");
                }
                catch
                {
                    MessageBox.Show("File could not be loaded!", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
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
                volumeButton.Background = this.loadImage("../Images/MuteCommu.png");
            }
            else
            {
                volumeBar.Value = _savedVolume;
                volumeButton.Background = this.loadImage("../Images/volumeCommu.png");
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
                loopAllButton.Background = this.loadImage("../Images/loopActivCommu.png");
            else
                loopAllButton.Background = this.loadImage("../Images/LoopAllCommu.png");
            if (this._isLoopSingle)
                LoopSingleButton.Background = this.loadImage("../Images/loopOneActiveCommu.png");
            else
                LoopSingleButton.Background = this.loadImage("../Images/LoopOneCommu.png");
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

        private void videoProgressBar_ValueChanged(object sender, DragCompletedEventArgs e)
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

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            this._isShuffle = !this._isShuffle;
            if (this._isShuffle)
            {
                this._listIndex = this._playList.shuffle(this._listIndex);
                Random.Background = this.loadImage("../Images/shuffleActiveCommu.png");
                
            }
            else
            {
                this._listIndex = this._playList.resetPlayList(this._listIndex);
                Random.Background = this.loadImage("../Images/ShuffleCommu.png");
            }
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.stopMedia();
            if (this._isLoopSingle)
            {
                this.playMedia();
                IMedia currentMedia = this._playList.getMediaByIndex(this._listIndex);
                if (currentMedia.Type == mediaType.AUDIO)
                    this.displayAudioElements(currentMedia);
            }
            else
                this.goNextMedia();
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                IMedia currentMedia = this._playList.getMediaByIndex(this._listIndex);
                if ((mediaElement.HasAudio || mediaElement.HasVideo) && currentMedia.Type != mediaType.IMAGE)
                {
                    System.TimeSpan total = this.mediaElement.NaturalDuration.TimeSpan;
                    this._timeDurationSize = 2;
                    this.videoProgressBar.Maximum = total.TotalSeconds;
                    this.totalTimeLabel.Content = this.formatTime(total);
                    this.currentTimeLabel.Content = this.formatTime(new System.TimeSpan(0, 0, 0));
                    GridProgressBar.Visibility = System.Windows.Visibility.Visible;
                    this._timeDurationSize = total.Hours > 0 ? 3 : 2;
                }
                else
                    GridProgressBar.Visibility = System.Windows.Visibility.Hidden;
                mediaTitle.Content = this._playList.getMediaTitle(this._listIndex);
                if (currentMedia.Type == mediaType.AUDIO)
                    this.displayAudioElements(currentMedia);
            }
            catch
            {
                System.Console.WriteLine("error");
            }
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message, "Media Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
        }

        private void mediaElementBackground_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            this._playList.addFolder(files);
            this.playList.ItemsSource = this._playList.getPlayList();
            if (mediaElement.Source == null && this._playList.getPlayList().Count() > 0)
            {
                mediaElement.Source = new Uri(this._playList.getMediaPath(0));
                this.playMedia();
            }
            this._isShuffle = false;
            Random.Background = this.loadImage("../Images/ShuffleCommu.png");
        }

        void synchronizeProgressBar(object sender, EventArgs e)
        {
            if ((mediaElement.HasVideo || mediaElement.HasAudio) && !_isPause)
            {
                double pos = mediaElement.Position.TotalSeconds;
                if (pos > videoProgressBar.Value || pos == 0)
                    videoProgressBar.Value = pos;
                this.currentTimeLabel.Content = this.formatTime(new System.TimeSpan(0, 0, (int)mediaElement.Position.TotalSeconds));
            }
        }

        //PLAYLISTS

        private void SetPlayList(string dir, string[] str)
        {
            System.Console.WriteLine("Set playlist");
            _pathList.Clear();
            for (int i = 0; i < str.Length; ++i)
            {
                _pathList.Add(dir + str[i]);
            }
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
                this.YoutubeEmbededPlayer.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Le lien n'est pas valide !", "Url Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
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
            this.YoutubeEmbededPlayer.Visibility = Visibility.Hidden;
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (YoutubeLink.Text == "Insert Youtube URL here")
                YoutubeLink.Foreground = new SolidColorBrush(Colors.Gray);
        }

        // CAMERA 

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.SaveImageCapture((BitmapSource)captureImage.Source);
        }

        private void CamCaptureTab_GotFocus(object sender, RoutedEventArgs e)
        {
            _webcam.Start();
            _webcam.Continue();
        }

        private void CamCaptureTab_LostFocus(object sender, RoutedEventArgs e)
        {
            _webcam.Stop();
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (!this._fullScreen)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.FullScreen.Background = this.loadImage("../Images/fullscreenActiveCommu.png");
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.FullScreen.Background = this.loadImage("../Images/fullScreenCommu.png");
            }
            this._fullScreen = !this._fullScreen;
        }

        //
        // TreeView
        //

        private void treeView1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = SearchTreeViewItem(e.OriginalSource as DependencyObject);

            if (item != null)
            {
                ContextMenu context = null;

                item.IsSelected = true;
                if ((item.Tag as string) == "LibraryItem")
                    context = this.treeView1.FindResource("LibraryItemMenu") as ContextMenu;
                else if ((item.Tag as string) == "Playlist")
                    context = this.treeView1.FindResource("PlaylistMenu") as ContextMenu;
                else if ((item.Tag as string) == "PlaylistFolder")
                    context = this.treeView1.FindResource("FolderMenu") as ContextMenu;
                else if ((item.Tag as string) == "PlaylistRoot")
                    context = this.treeView1.FindResource("RootMenu") as ContextMenu;
                if (context != null)
                {
                    context.PlacementTarget = this;
                    context.IsOpen = true;
                }
            }
        }

        private TreeViewItem SearchTreeViewItem(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void AddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.treeView1.SelectedItem as TreeViewItem;
            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter playlist's name below :", "Playlist Creation", "new playlist", 0, 0);
            
            if (this._playlistManager.AddPlaylistToFolder(this._users.getLoggedUser(), name, (item.Header as string)))
                item.Items.Add(new TreeViewItem { Header = name, Tag = "Playlist" });
        }

        private void SelectPlaylist_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.treeView1.SelectedItem as TreeViewItem;
            TreeViewItem folder = ItemsControl.ItemsControlFromItemContainer(item) as TreeViewItem;
            
            this._playList.selectPlaylist(this._playlistManager.selectPlaylistInFolder(this._users.getLoggedUser(), item.Header as string, folder.Header as string));
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.treeView1.SelectedItem as TreeViewItem;
            string name = Microsoft.VisualBasic.Interaction.InputBox("Enter folder's name below :", "Folder Creation", "new folder", 0, 0);
            
            if (this._playlistManager.AddFolder(this._users.getLoggedUser(), name))
                item.Items.Add(new TreeViewItem { Header = name, Tag = "PlaylistFolder" });
        }

        private void DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.treeView1.SelectedItem as TreeViewItem;
            TreeViewItem root = treeView1.Items[0] as TreeViewItem;

            if (this._playlistManager.removeFolder(this._users.getLoggedUser(), item.Header as string))
                root.Items.Remove(item);
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.treeView1.SelectedItem as TreeViewItem;
            TreeViewItem folder = ItemsControl.ItemsControlFromItemContainer(item) as TreeViewItem;

            if (this._playlistManager.removePlaylistFromFolder(this._users.getLoggedUser(), item.Header as string, folder.Header as string))
                folder.Items.Remove(item);
        }

        private void reloadTreeView()
        {
            TreeViewItem root = treeView1.Items[0] as TreeViewItem;
            List<TreeViewItem> lst = this._playlistManager.reload(this._users.getLoggedUser());

            root.Items.Clear();
            foreach (TreeViewItem item in lst)
                root.Items.Add(item);
        }

        private void ReloadTreeView_Click(object sender, RoutedEventArgs e)
        {
            reloadTreeView();
            MessageBox.Show("Reload done", "Playlist info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
        }

        //
        // Other
        //

        private void CreateUserBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            this._users.addUser(this.userNameTbx.Text, this.passwordPbx.Password);
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this._users.checkUser(this.userNameTbx.Text, this.passwordPbx.Password))
            {
                this.ProfileUserNameTBx.Text = this._users.getLoggedUser().UserName;
                this.Login.Visibility = Visibility.Hidden;
                this.Profile.Visibility = Visibility.Visible;
                reloadTreeView();
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            this._users.logoutUser();
            this.Profile.Visibility = Visibility.Hidden;
            this.Login.Visibility = Visibility.Visible;
            reloadTreeView();
        }

        private void audioAnimationMediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("fail to open animation", "Animation Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
        }

        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    string header = headerClicked.Column.Header as string;
                    _playList.sortPlaylist(header);
                }
            }
        }

        private void searchPlaylist_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchPlaylist.Text == "Search in playlist")
                searchPlaylist.Text = "";
            searchPlaylist.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void searchPlaylist_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchPlaylist.Text == "")
                searchPlaylist.Text = "Search in playlist";
            searchPlaylist.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void searchPlaylist_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchPlaylist.Text != "Search in playlist")
                this._playList.searchInPlaylist(searchPlaylist.Text);
        }

        private void playList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                ListViewItem Item = (ListViewItem)sender;
                IMedia media = (IMedia)Item.Content;

                this._listIndex = this._playList.findMediaIndex(media);
                this.loadAndPlayMedia();
                e.Handled = true;
                mediaTab.Focus();
            }
        }

        private void ChangeUserName_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to change your UserName?", "Changing User Name Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                this._users.changeUserName(this.ProfileUserNameTBx.Text);
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to change your password?", "Changing User Name Warning", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                this._users.changePassword(this.ProfileOldPasswordTBx.Password, this.ProfileNewPassword1TBx.Password, this.ProfileNewPassword2TBx.Password);
                this.ProfileOldPasswordTBx.Password = "";
                this.ProfileNewPassword1TBx.Password = "";
                this.ProfileNewPassword2TBx.Password = "";
            }
        }

        private void playList_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            this._playList.addFolder(files);
            this.playList.ItemsSource = this._playList.getPlayList();
            if (mediaElement.Source == null && this._playList.getPlayList().Count() > 0)
            {
                mediaElement.Source = new Uri(this._playList.getMediaPath(0));
                this.playMedia();
            }
            this._isShuffle = false;
            Random.Background = this.loadImage("../Images/ShuffleCommu.png");
        }
    }
}
