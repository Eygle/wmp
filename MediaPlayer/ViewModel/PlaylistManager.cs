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
using System.Xml.Serialization;

namespace MediaPlayer.ViewModel
{
    public class PlaylistManager
    {
        public static string PlaylistPath = "Playlists/";
        public static string LibraryPath = "Libbrary/";

        private Dictionary<string, Playlist>         _playlists;
        private Dictionary<string, List<string>>    _tree;

        static private Regex _regexName = new Regex("^[a-z|0-9|\\s]+$", RegexOptions.IgnoreCase);

        public static void save(Playlist toSave)
        {
            using (FileStream fs = new FileStream(toSave.path, FileMode.Create))
            {
                //toSave.setSavablePlaylist();
                XmlSerializer serializer = new XmlSerializer(typeof(Model.Playlist));
                serializer.Serialize(fs, toSave);
            }
        }

        public static Playlist load(string pathname)
        {
            Playlist pls;

            using (FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Model.Playlist));
                pls = serializer.Deserialize(fs) as Model.Playlist;
            }
            return pls;
        }

        public PlaylistManager() 
        {
            this._tree = new Dictionary<string, List<string>>();
            this._playlists = new Dictionary<string, Playlist>();
            if (!Directory.Exists(PlaylistPath))
                Directory.CreateDirectory(PlaylistPath);
            if (!Directory.Exists(LibraryPath))
                Directory.CreateDirectory(LibraryPath);
        }

        private void    loadPlaylist(string path)
        {
            Playlist pls;
            path = path.Replace("\\", "/");
            try
            {
                pls = PlaylistManager.load(path);
            }
            catch
            {
                pls = new Playlist(path);
                PlaylistManager.save(pls);
            }
            this._playlists.Add(path, pls);
        }

        public Playlist selectPlaylistInFolder(User user, string name, string folder)
        {
            string pls = PlaylistPath + user.UserName + "/" + folder + "/" + name;
            return (this._playlists[pls]);
        }

        public bool AddFolder(User user, string name)
        {
            if (name == "")
                return false;
            else if (user == null)
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
                string pls = PlaylistPath + user.UserName + "/" + folder + "/" + name;
                File.Delete(pls);
                this._tree[folder].Remove(name);
                this._playlists.Remove(pls);
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
            if (name == "")
                return false;
            else if (this._tree[folder].Contains(name) || !_regexName.Match(name).Success)
            {
                MessageBox.Show("playlist's name invalid", "Playlist Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return false;
            }
            string pls = PlaylistPath + user.UserName + "/" + folder + "/" + name;
            FileStream fs = File.Create(pls);
            fs.Close();
            this._tree[folder].Add(name);
            this.loadPlaylist(pls);
            return true;
        }

        public List<TreeViewItem> reload(User user)
        {
            List<TreeViewItem> res = new List<TreeViewItem>();

            this._tree.Clear();
            this._playlists.Clear();
            res.Add(new TreeViewItem { Header = "default playlist", Tag = "LibraryItem" });
            if (user != null)
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
                        this.loadPlaylist(file);
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
