﻿using System;
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
            Directory.CreateDirectory(PlaylistPath + "toto" + "/" + name); // TODO: don't hardcode the username
            this._tree.Add(name, new List<string>());
            return true;
        }

        public bool AddPlaylistToFolder(string name, string folder)
        {
            if (this._tree[folder].Contains(name) || !_regexName.Match(name).Success)
                return false;
            File.Create(PlaylistPath + "toto" + "/" + folder + "/" + name); // TODO: don't hardcode the username
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
    }
}
