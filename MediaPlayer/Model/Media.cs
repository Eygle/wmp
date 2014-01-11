using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Model
{
    [Serializable]
    public class Media
    {
        public enum Type
        {
            VIDEO,
            IMAGE,
            AUDIO
        };

        public int length;
        public string name;
        public string path;
        public Type type;

        public Media() { }

        public Media(int length, string mediaName, string path, Type type)
        {
            this.length = length;
            this.name = mediaName;
            this.path = path;
            this.type = type;
        }
    }
}
