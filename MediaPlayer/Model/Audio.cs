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
        private string _lengthString;
        private long _lengthLong;
        private string _title;
        private string _genre;
        private string _path;
        private string _fileSize;
        private string _artist;
        private string _year;
        private mediaType type;

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
            this._path = path;
            Dictionary<int, KeyValuePair<string, string>> fileProps = GetFileProps(path);
            /*foreach (KeyValuePair<int, KeyValuePair<string, string>> kv in fileProps)
            {
                Console.WriteLine(kv.ToString());
            }*/
            this._lengthString = fileProps[27].Value;
            this._title = fileProps[21].Value;
            this._fileSize = fileProps[1].Value;
            this._artist = fileProps[20].Value;
            this._year = fileProps[15].Value;
            this._genre = null;
            long[] multipliers = new long[] { 3600, 60, 1 };
            int i = 0;
            this._lengthLong = this._lengthString.Split(':').Aggregate(0, (long total, string part) => total += Int64.Parse(part) * multipliers[i++]);
        
            type = mediaType.AUDIO;
        }

        string IMedia.getTitle()
        {
            return this._title;
        }

        string IMedia.getPath()
        {
            return this._path;
        }

        mediaType IMedia.getType()
        {
            return this.type;
        }

        string IMedia.getLengthString()
        {
            return this._lengthString;
        }

        long IMedia.getLengthLong()
        {
            return this._lengthLong;
        }

        string IMedia.getGenre()
        {
            return this._genre;
        }

        string IMedia.getFileSize()
        {
            return this._fileSize;
        }
    }
}
