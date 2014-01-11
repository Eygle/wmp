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
        private string _lengthString;
        private long _lengthLong;
        private string _title;
        private string _genre;
        private string _path;
        private string _fileSize;
        private mediaType type;

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
            this._path = path;

            Dictionary<int, KeyValuePair<string, string>> fileProps = GetFileProps(path);
            /*foreach (KeyValuePair<int, KeyValuePair<string, string>> kv in fileProps)
            {
                Console.WriteLine(kv.ToString());
            }*/
            this._lengthString = null;
            this._lengthLong = 0;
            this._title = fileProps[0].Value;
            this._fileSize = fileProps[1].Value;
            this._genre = null;
            type = mediaType.IMAGE;
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
