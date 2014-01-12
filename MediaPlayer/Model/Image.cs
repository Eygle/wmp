using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shell32;

namespace MediaPlayer.Model
{
    [Serializable]
    public class mediaImage : IMedia
    {
        public string LengthString { get; set; }
        public long LengthLong { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string PathName { get; set; }
        public string FileSize { get; set; }
        public mediaType Type { get; set; }

        public mediaImage() { }

        Dictionary<int, KeyValuePair<string, string>> GetFileProps(string filename)
        {
            Shell shl = new Shell();
            Folder fldr = shl.NameSpace(Path.GetDirectoryName(filename));
            FolderItem itm = fldr.ParseName(Path.GetFileName(filename));
            Dictionary<int, KeyValuePair<string, string>> fileProps = new Dictionary<int, KeyValuePair<string, string>>();
            for (int i = 0; i < 100; i++)
            {
                string propValue = fldr.GetDetailsOf(itm, i);
                if (propValue != "")
                {
                    fileProps.Add(i, new KeyValuePair<string, string>(fldr.GetDetailsOf(null, i), propValue));
                }
            }
            return fileProps;
        }

        public mediaImage(string path)
        {
            this.PathName = path;

            Dictionary<int, KeyValuePair<string, string>> fileProps = GetFileProps(path);
            /*foreach (KeyValuePair<int, KeyValuePair<string, string>> kv in fileProps)
            {
                Console.WriteLine(kv.ToString());
            }*/
            this.LengthString = null;
            this.LengthLong = 0;
            this.Title = fileProps[0].Value;
            this.FileSize = fileProps[1].Value;
            this.Genre = null;
            Type = mediaType.IMAGE;
        }
    }
}
