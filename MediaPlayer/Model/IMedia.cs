using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace MediaPlayer.Model
{
    [Serializable]
    public enum mediaType
    {
        VIDEO,
        IMAGE,
        AUDIO
    };

    [XmlInclude(typeof(Video))]
    [XmlInclude(typeof(Audio))]
    [XmlInclude(typeof(Image))]
    public abstract class IMedia
    {
        public abstract string Title { get; set; }
        public abstract mediaType Type { get; set; }
        public abstract string LengthString { get; set; }
        public abstract long LengthLong { get; set; }
        public abstract string Genre { get; set; }
        public abstract string Album { get; set; }
        public abstract string PathName { get; set; }
        public abstract string FileSize { get; set; }
        public abstract string Year { get; set; }
        public abstract string Artist { get; set; }
        public abstract long Size { get; set; }
        //public abstract BitmapImage Icon { get; set; }
    }
}
