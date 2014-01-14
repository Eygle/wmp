using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MediaPlayer.ViewModel
{
    public class PlaylistManager
    {
        public static string PlaylistPath = "Playlists/";
        public static string LibraryPath = "Libbrary/";

        private Dictionary<string, List<string>> _tree;
        static private Regex _regexName = new Regex("^[a-z|0-9|\\s]+$", RegexOptions.IgnoreCase);

        public PlaylistManager() 
        {
            this._tree = new Dictionary<string, List<string>>();
            if (!Directory.Exists(PlaylistPath))
                Directory.CreateDirectory(PlaylistPath);
            if (!Directory.Exists(LibraryPath))
                Directory.CreateDirectory(LibraryPath);
        }

        public bool AddFolder(User user, string name)
        {
            if (user == null)
            {
                MessageBox.Show("You must be logged to create folders", "Playlist Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }            
            else if (name == "" || this._tree.ContainsKey(name) || !_regexName.Match(name).Success)
            {
                MessageBox.Show("Folder's name invalid", "Playlist Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
            Directory.CreateDirectory(PlaylistPath + user.UserName + "/" + name);
            this._tree.Add(name, new List<string>());
            return true;
        }

        public bool removeFolder(User user, string name)
        {
            try
            {
                Directory.Delete(PlaylistPath + user.UserName + "/" + name, true);
                this._tree.Remove(name);
                return true;
            }
            catch
            {
                MessageBox.Show("Can't delete folder", "Playlist Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
        }
        public bool removePlaylistFromFolder(User user, string name, string folder)
        {
            try
            {
                if (MessageBox.Show("Do you really want to delete this playlist ?", "Playlist Change", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return false;
                File.Delete(PlaylistPath + user.UserName + "/" + folder + "/" + name);
                this._tree[folder].Remove(name);
                return true;
            }
            catch
            {
                MessageBox.Show("Can't delete playlist", "Playlist Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
        }

        public bool AddPlaylistToFolder(User user, string name, string folder)
        {
            if (name == "" || this._tree[folder].Contains(name) || !_regexName.Match(name).Success)
            {
                MessageBox.Show("playlist's name invalid", "Playlist Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
            File.Create(PlaylistPath + user.UserName + "/" + folder + "/" + name);
            this._tree[folder].Add(name);
            return true;
        }

        public List<TreeViewItem> reload(User user)
        {
            List<TreeViewItem> res = new List<TreeViewItem>();

            this._tree.Clear();
            res.Add(new TreeViewItem { Header = "default playlist", Tag = "LibraryItem" });
            if (user.UserName != "")
            {
                string[] dirs = Directory.GetDirectories(PlaylistPath + user.UserName);

                foreach (string dir in dirs)
                {
                    string[] files = Directory.GetFiles(dir);
                    string folder = Path.GetFileName(dir);
                    TreeViewItem item = new TreeViewItem { Header = folder, Tag = "PlaylistFolder" };

                    this._tree.Add(folder, new List<string>());
                    res.Add(item);
                    foreach (string file in files)
                    {
                        string pls = Path.GetFileName(file);
                        this._tree[folder].Add(pls);
                        item.Items.Add(new TreeViewItem { Header = pls, Tag = "Playlist" });
                    }
                }
            }
            return res;
        }

        public void fillLibrary(List<Playlist> list)
        {
            Playlist audio = new Playlist();
            Playlist video = new Playlist();
            Playlist image = new Playlist();
            foreach (Playlist pl in list)
            {
                foreach (IMedia m in pl.getPlayList())
                {
                    if (m.Type == mediaType.AUDIO)
                        audio.add(m);
                    else if (m.Type == mediaType.VIDEO)
                        video.add(m);
                    else if (m.Type == mediaType.IMAGE)
                        image.add(m);
                }
            }
        }
    }
}
