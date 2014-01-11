using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MediaPlayer.Model
{
    [Serializable]
    public class Playlist
    {
        [XmlElement(ElementName = "PlaylistName")]
        private string _name;
        [XmlElement(ElementName = "TotalLength")]
        private int _totalLength;
        [XmlElement(ElementName = "NumberElements")]
        private int _size;
        [XmlElement(ElementName = "Content")]
        private List<Media> _content;

        public Playlist()
        {
            this._name = "default";
            this._content = new List<Media>();
        }

        public Playlist(string playlistName, List<Media> content)
        {
            this._name = playlistName;
            this._content = content;
        }

        public void clear()
        {
            this._content.Clear();
            this._size = 0;
            this._totalLength = 0;
        }

        public void add(Media item)
        {
            this._content.Add(item);
            this._size++;
            this._totalLength += item.length;
        }

        public void remove(Media item)
        {
            this._size--;
            this._totalLength -= item.length;
            this._content.Remove(item);
        }
    }
}
