using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shell32;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace MediaPlayer.Model
{
    [Serializable]
    public class Image : IMedia
    {
        private string _lengthString;
        private long _lengthLong;
        private string _title;
        private string _genre;
        private string _album;
        private string _pathName;
        private string _fileSize;
        private string _artist;
        private string _year;
        private mediaType _type;
        private BitmapImage _icon;

        public Image() { }

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

        public Image(string path)
        {
            this.PathName = path;

            Dictionary<int, KeyValuePair<string, string>> fileProps = GetFileProps(path);
            foreach (KeyValuePair<int, KeyValuePair<string, string>> kv in fileProps)
            {
                Console.WriteLine(kv.ToString());
            }
            this._lengthString = null;
            this._lengthLong = 0;
            this._title = fileProps[0].Value;
            this._fileSize = fileProps[1].Value;
            this._genre = null;
            this._type = mediaType.IMAGE;
            this._icon = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "/../../Images/photo.ico"));
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
            set { this._album = value; }
        }

        public BitmapImage Icon
        {
            get { return this._icon; }
            set { this._icon = value; }
        }
    }
}
