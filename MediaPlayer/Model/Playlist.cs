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
        private string _name;
        [XmlElement(ElementName = "TotalLength")]
        private long _totalLength;
        [XmlElement(ElementName = "NumberElements")]
        private int _size;
        [XmlElement(ElementName = "Content")]
        private List<IMedia> _content;

        public Playlist(string name = "default")
        {
            this._name = name;
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
