using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MediaPlayer.ViewModel
{
    class CurrentPlaylist
    {
        private ObservableCollection<IMedia> _oc = new ObservableCollection<IMedia>();
        private Model.Playlist _playlist;

        public CurrentPlaylist()
        {
            this._playlist = new Model.Playlist();
        }

        public CurrentPlaylist(CurrentPlaylist o)
        {
            this._playlist = o._playlist;
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
                    medias.Add(m);
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
                    medias.Add(m);
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

        public ObservableCollection<IMedia> getPlayList()
        {
            _oc.Clear();
            foreach (IMedia item in this._playlist.getPlayList())
                _oc.Add(item);
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
            foreach (IMedia item in this._playlist.getPlayList())
                _oc.Add(item);
        }

        public IMedia getMediaByIndex(int index)
        {
            return this._oc[index];
        }

        private void addToPlaylist(List<IMedia> items)
        {
            foreach (IMedia item in items)
                this._playlist.add(item);
            _oc.Clear();
            this.getPlayList();
        }
    }
}
