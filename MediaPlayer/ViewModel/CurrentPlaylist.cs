using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using MediaPlayer;

namespace MediaPlayer.ViewModel
{
    class CurrentPlaylist
    {
        public enum direction
        {
            ASC,
            DESC
        };

        private ObservableCollection<IMedia> _oc = new ObservableCollection<IMedia>();
        private Model.Playlist _playlist;
        private delegate void sortPlaylistFunc();
        private Dictionary<string, Func<bool>> _sortPlaylistsFuncs = new Dictionary<string, Func<bool>>();
        private string _lastSortHeader = "";
        private direction _lastDirection;

        public CurrentPlaylist()
        {
            this._playlist = new Model.Playlist();
            this._sortPlaylistsFuncs["Title"] = sortByTitle;
            this._sortPlaylistsFuncs["File Size"] = sortBySize;
            this._sortPlaylistsFuncs["Time"] = sortByTime;
            this._sortPlaylistsFuncs["Genre"] = sortByGenre;
            this._sortPlaylistsFuncs["Type"] = sortByType;
        }

        public CurrentPlaylist(CurrentPlaylist o)
        {
            this._playlist = o._playlist;
        }

        ~CurrentPlaylist()
        {
            // TODO: call save method
        }

        public void save(string name)
        {
            using (FileStream fs = new FileStream(name, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Model.Playlist));
                serializer.Serialize(fs, this._playlist);
            }
        }

        public void load(string pathname)
        {
            using (FileStream fs = new FileStream(pathname, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Model.Playlist));
                this._playlist = serializer.Deserialize(fs) as Model.Playlist;
            }
        }

        public void addFiles(string pathName, string[] names)
        {
            List<string> fullPaths = new List<string>();
            for (int i = 0; i < names.Count(); ++i)
                fullPaths.Add(pathName + names[i]);
            MediaFactory f = new MediaFactory();
            List<IMedia> medias = new List<IMedia>();
            foreach (string fullPath in fullPaths)
            {
                    IMedia m = f.createMedia(fullPath);
                    if (m != null)
                    {
                        _oc.Add(m);
                        medias.Add(m);
                    }
            }
           this.addToPlaylist(medias);
        }

        public void addFolder(string[] pathNames)
        {
            MediaFactory f = new MediaFactory();
            List<IMedia> medias = new List<IMedia>();
            for (int i = 0; i < pathNames.Count(); ++i)
            {
                IMedia m = f.createMedia(pathNames[i]);
                if (m != null)
                {
                    _oc.Add(m);
                    medias.Add(m);
                }
            }
            this.addToPlaylist(medias);
        }

        public string getMediaPath(int index)
        {
            return this._playlist.getMediaAtIndex(index).PathName;
        }


        public string getMediaTitle(int index)
        {
            return this._playlist.getMediaAtIndex(index).Title;
        }

        public void resetPlayList()
        {
            _oc.Clear();
            foreach (IMedia item in this._playlist.getPlayList())
                _oc.Add(item);
        }

        public ObservableCollection<IMedia> getPlayList()
        {
            return _oc;
        }

        public int Count()
        {
            return this._playlist.count();
        }

        public void shuffle()
        {
            Random rnd = new Random();
            List<IMedia> mediaList =  this._playlist.getPlayList().OrderBy(x => rnd.Next()).ToList();
            _oc.Clear();
            foreach (IMedia item in mediaList)
                _oc.Add(item);
        }

        private bool sortByTitle()
        {
            List<IMedia> mediaList;
            if (this._lastDirection == direction.ASC)
                mediaList = this._playlist.getPlayList().OrderBy(m => m.Title).ToList();
            else
                mediaList = this._playlist.getPlayList().OrderByDescending(m => m.Title).ToList();
             _oc.Clear();
             foreach (IMedia item in mediaList)
                 _oc.Add(item);
             return true;
        }

        private bool sortByTime()
        {
            List<IMedia> mediaList;
            if (this._lastDirection == direction.ASC)
                mediaList = this._playlist.getPlayList().OrderBy(m => m.LengthLong).ToList();
            else
                mediaList = this._playlist.getPlayList().OrderByDescending(m => m.LengthLong).ToList();
            _oc.Clear();
            foreach (IMedia item in mediaList)
                _oc.Add(item);
            return true;
        }

        private bool sortByGenre()
        {
            List<IMedia> mediaList;
            if (this._lastDirection == direction.ASC)
                mediaList = this._playlist.getPlayList().OrderBy(m => m.Genre).ToList();
            else
                mediaList = this._playlist.getPlayList().OrderByDescending(m => m.Genre).ToList();
            _oc.Clear();
            foreach (IMedia item in mediaList)
                _oc.Add(item);
            return true;
        }

        private bool sortBySize()
        {
            List<IMedia> mediaList;
            if (this._lastDirection == direction.ASC)
                mediaList = this._playlist.getPlayList().OrderBy(m => m.Size).ToList();
            else
                mediaList = this._playlist.getPlayList().OrderByDescending(m => m.Size).ToList();
            _oc.Clear();
            foreach (IMedia item in mediaList)
                _oc.Add(item);
            return true;
        }

        private bool sortByType()
        {
            List<IMedia> mediaList;
            if (this._lastDirection == direction.ASC)
                mediaList = this._playlist.getPlayList().OrderBy(m => m.Type).ToList();
            else
                mediaList = this._playlist.getPlayList().OrderByDescending(m => m.Type).ToList();
            _oc.Clear();
            foreach (IMedia item in mediaList)
                _oc.Add(item);
            return true;
        }

        public IMedia getMediaByIndex(int index)
        {
            return this._oc[index];
        }

        private void addToPlaylist(List<IMedia> items)
        {
            foreach (IMedia item in items)
            {
                this._playlist.add(item);
            }
            this.resetPlayList();
        }

        public void sortPlaylist(string header)
        {
            direction dir = direction.ASC;
            if (header == _lastSortHeader)
                dir = this._lastDirection == direction.ASC ? direction.DESC : direction.ASC;
            this._lastDirection = dir;
            this._lastSortHeader = header;
            _sortPlaylistsFuncs[header]();
        }

        public void searchInPlaylist(string text)
        {
            List<IMedia> mediaList = this._playlist.getPlayList().Where(m => m.Title.ToLower().Contains(text.ToLower())).ToList();
            _oc.Clear();
            foreach (IMedia item in mediaList)
                _oc.Add(item);
        }
    }
}
