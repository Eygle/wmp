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
        private long   _lengthLong;
        private string _title;
        private string _genre;
        private string _album;
        private string _pathName;
        private string _fileSize;
        private string _artist;
        private string _year;
        private mediaType _type;

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
            this._pathName = path;
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
            this._genre = fileProps[16].Value;
            this._album = fileProps[14].Value;
            long[] multipliers = new long[] { 3600, 60, 1 };
            int i = 0;
            this._lengthLong = this._lengthString.Split(':').Aggregate(0, (long total, string part) => total += Int64.Parse(part) * multipliers[i++]);
            _type = mediaType.AUDIO;
        }

        public string LengthString
        {
            get { return this._lengthString; }
            set { this._lengthString = value; }
        }

        public long LengthLong
        {
            get { return this._lengthLong; }
            set { this._lengthLong = value; }
        }

        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }
        public string Genre
        {
            get { return this._genre; }
            set { this._genre = value; }
        }

        public string PathName
        {
            get { return this._pathName; }
            set { this._pathName = value; }
        }

        public string FileSize
        {
            get { return this._fileSize; }
            set { this._fileSize = value; }
        }

        public string Artist
        {
            get { return this._artist; }
            set { this._artist = value; }
        }

        public string Year
        {
            get { return this._year; }
            set { this._year = value; }
        }

        public mediaType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        public string Album
        {
            get { return this._album; }
            set { this._album = value;  }
        }
    }
}
