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
        private List<string> _playlists;

        public PlaylistManager() 
        { 
            this._playlists = new List<string>();
        }

        public void AddFolder(string name)
        {
        }

        public bool AddPlaylist(string name)
        {
            Regex r = new Regex("^[a-z|0-9|\\s]+$", RegexOptions.IgnoreCase);

            if (this._playlists.Contains(name) || !r.Match(name).Success)
                return (false);
            this._playlists.Add(name); // TODO: think about the path
            return (true);
        }

        public void AddFiles(string playlist, string[] path)
        {
        }
    }
}
