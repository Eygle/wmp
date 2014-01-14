using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.ViewModel
{
    class MediaFactory
    {
        private delegate IMedia CreateMediaDelegate(string pathname);
        
        //private Dictionary<string, CreateMediaDelegate> _allowedExt; // TODO: use this instead of the code below
        private string[] _allowedExt = { ".mp3", ".mp4", ".asf", ".3gp", ".3g2", ".asx", ".avi", ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".wma", ".mov" };
        private string[] _videoExt = { ".mp4", ".asf", ".3gp", ".3g2", ".asx", ".avi", ".mov", ".wma"};
        private string[] _imageExt = { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };

        public MediaFactory()
        {
        }

        public IMedia createMedia(string pathName)
        {
            if (!this._allowedExt.Contains(System.IO.Path.GetExtension(pathName)))
            {
                Console.Out.WriteLine("Error tmp");
                return null;
            }

            if (this._videoExt.Contains(System.IO.Path.GetExtension(pathName)))
                return new Video(pathName);
            else if (this._imageExt.Contains(System.IO.Path.GetExtension(pathName)))
                return new mediaImage(pathName);
            return new Audio(pathName);
        }
    }
}
