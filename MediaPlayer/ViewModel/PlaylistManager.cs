using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        }

        public bool AddFolder(string username, string name)
        {
            if (this._tree.ContainsKey(name) || !_regexName.Match(name).Success)
                return false;
            Directory.CreateDirectory(PlaylistPath + username + "/" + name);
            this._tree.Add(name, new List<string>());
            return true;
        }

        public bool removeFolder(string username, string name)
        {
            try
            {
                Directory.Delete(PlaylistPath + username + "/" + name, true);

                this._tree.Remove(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddPlaylistToFolder(string username, string name, string folder)
        {
            if (this._tree[folder].Contains(name) || !_regexName.Match(name).Success)
                return false;
            File.Create(PlaylistPath + username + "/" + folder + "/" + name);
            this._tree[folder].Add(name);
            return true;
        }

        public Dictionary<string, List<string>> reload(string username)
        {
            this._tree.Clear();
            if (username == "")
                return null;
            string[] dirs = Directory.GetDirectories(PlaylistPath + username);
            foreach (string dir in dirs)
            {
                string[] files = Directory.GetFiles(dir);
                string folder = Path.GetFileName(dir);
                this._tree.Add(folder, new List<string>());
                foreach (string pls in files)
                    this._tree[folder].Add(Path.GetFileName(pls));
            }
            return this._tree;
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
