using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MediaPlayer.ViewModel
{
    class CurrentPlaylist
    {
        private Model.Playlist _playlist;

        public CurrentPlaylist()
        {
            this._playlist = new Model.Playlist();
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
                medias.Add(f.createMedia(fullPath));
            this.addToPlaylist(medias);
        }

        public void addFolder(string[] pathNames)
        {
            MediaFactory f = new MediaFactory();
            List<IMedia> medias = new List<IMedia>();
            for (int i = 0; i < pathNames.Count(); ++i)
                medias.Add(f.createMedia(pathNames[i]));
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

        public List<IMedia> getPlayList()
        {
            return this._playlist.getPlayList();
        }

        public int Count()
        {
            return this._playlist.count();
        }

        public void shuffle()
        {
            this._playlist.shuffle();
        }

        private void addToPlaylist(List<IMedia> items)
        {
            foreach (IMedia item in items)
                this._playlist.add(item);
        }
    }
}
