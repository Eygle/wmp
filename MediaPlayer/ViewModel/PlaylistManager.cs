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

        private List<string> _playlists;

        public PlaylistManager() 
        { 
            this._playlists = new List<string>();
            if (!Directory.Exists(PlaylistPath))
                Directory.CreateDirectory(PlaylistPath);
        }

        public bool AddFolder(string name)
        {
            return false;
        }

        public bool AddPlaylist(string name)
        {
            Regex r = new Regex("^[a-z|0-9|\\s]+$", RegexOptions.IgnoreCase);

            if (this._playlists.Contains(name) || !r.Match(name).Success)
                return false;
            this._playlists.Add(name); // TODO: think about the path
            return true;
        }

        public void AddFiles(string playlist, string[] path)
        {
        }
    }
}
