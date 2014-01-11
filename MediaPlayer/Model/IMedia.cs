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
        string getTitle();
        mediaType getType();
        string getLengthString();
        long getLengthLong();
        string getGenre();
        string getPath();
        string getFileSize();
    }
}
