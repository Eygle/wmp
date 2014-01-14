using System;
using System.Collections.Generic;
using System.IO;
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
        public string _name;
        [XmlElement(ElementName = "TotalLength")]
        public long _totalLength;
        [XmlElement(ElementName = "NumberElements")]
        public int _size;
        //[XmlElement(ElementName = "Content")]
        [XmlIgnore()]
        public List<IMedia> _content;
        [XmlIgnore()]
        public string path;

        public Playlist()
        {
            string path = "Playlist/default";
            this.path = path;
            this._content = new List<IMedia>();
        }

        public Playlist(string path)
        {
            this._name = Path.GetFileName(path);
            this.path = path;
            this._content = new List<IMedia>();
        }

        public void clear()
        {
            this._content.Clear();
            this._size = 0;
            this._totalLength = 0;
        }

        public void add(IMedia item)
        {
            this._content.Add(item);
            this._size++;
            this._totalLength += item.LengthLong;
        }

        public void remove(IMedia item)
        {
            this._size--;
            this._totalLength -= item.LengthLong;
            this._content.Remove(item);
        }

        public IMedia getMediaAtIndex(int index)
        {
            return this._content[index];
        }

        public List<IMedia> getPlayList()
        {
            return this._content;
        }

        public int count()
        {
            return this._content.Count;
        }
    }
}
