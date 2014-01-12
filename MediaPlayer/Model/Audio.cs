using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MediaPlayer.Model
{
    [Serializable]
    public class Audio : IMedia
    {
        public string LengthString { get; set; }
        public long LengthLong { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string PathName { get; set; }
        public string FileSize { get; set; }
        public string Artist { get; set; }
        public string Year { get; set; }
        public mediaType Type { get; set; }

        public Audio() { }

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

        public Audio(string path)
        {
            this.PathName = path;
            Dictionary<int, KeyValuePair<string, string>> fileProps = GetFileProps(path);
            /*foreach (KeyValuePair<int, KeyValuePair<string, string>> kv in fileProps)
            {
                Console.WriteLine(kv.ToString());
            }*/
            this.LengthString = fileProps[27].Value;
            this.Title = fileProps[21].Value;
            this.FileSize = fileProps[1].Value;
            this.Artist = fileProps[20].Value;
            this.Year = fileProps[15].Value;
            this.Genre = null;
            long[] multipliers = new long[] { 3600, 60, 1 };
            int i = 0;
            this.LengthLong = this.LengthString.Split(':').Aggregate(0, (long total, string part) => total += Int64.Parse(part) * multipliers[i++]);
        
            Type = mediaType.AUDIO;
        }
    }
}
