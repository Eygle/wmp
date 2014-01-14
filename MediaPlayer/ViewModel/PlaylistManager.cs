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

        private Dictionary<string, List<string>> _tree;
        static private Regex _regexName = new Regex("^[a-z|0-9|\\s]+$", RegexOptions.IgnoreCase);

        public PlaylistManager() 
        {
            this._tree = new Dictionary<string, List<string>>();
            if (!Directory.Exists(PlaylistPath))
                Directory.CreateDirectory(PlaylistPath);
        }

        public bool AddFolder(string name)
        {
            if (this._tree.ContainsKey(name) || !_regexName.Match(name).Success)
                return false;
            this._tree.Add(name, new List<string>());
            return true;
        }

        public bool AddPlaylistToFolder(string name, string folder)
        {
            if (this._tree[folder].Contains(name) || !_regexName.Match(name).Success)
                return false;
            this._tree[folder].Add(name);
            return true;
        }

        public void AddFiles(string playlist, string[] path)
        {
        }
    }
}
