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
    }
}
