using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Model
{
    public enum mediaType
    {
        VIDEO,
        IMAGE,
        AUDIO
    };
    //[Serializable]
    public interface IMedia
    {
        string Title { get; set; }
        mediaType Type { get; set; }
        string LengthString { get; set; }
        long LengthLong { get; set; }
        string Genre { get; set; }
        string Album { get; set; }
        string PathName { get; set; }
        string FileSize { get; set; }
        string Year { get; set; }
        string Artist { get; set; }
    }
}
