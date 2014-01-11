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
        private long _totalLength;
        [XmlElement(ElementName = "NumberElements")]
        private int _size;
        [XmlElement(ElementName = "Content")]
        private List<IMedia> _content;

        public Playlist()
        {
            this._name = "default";
            this._content = new List<IMedia>();
        }

        public Playlist(string playlistName, List<IMedia> content)
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

        public void add(IMedia item)
        {
            this._content.Add(item);
            this._size++;
            this._totalLength += item.getLengthLong();
        }

        public void remove(IMedia item)
        {
            this._size--;
            this._totalLength -= item.getLengthLong();
            this._content.Remove(item);
        }

        public IMedia getMediaAtIndex(int index)
        {
            return this._content[index];
        }

        public int count()
        {
            return this._content.Count;
        }

        public void shuffle()
        {
            Random rnd = new Random();
            this._content = this._content.OrderBy(x => rnd.Next()).ToList();
        }
    }
}
