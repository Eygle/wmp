using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MediaPlayer.ViewModel
{
    class MediaFactory
    {
        private string[] _allowedExt = { ".mp3", ".mp4", ".asf", ".3gp", ".3g2", ".asx", ".avi", ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".wma", ".mov" };
        private string[] _videoExt = { ".mp4", ".asf", ".3gp", ".3g2", ".asx", ".avi", ".mov", ".wma"};
        private string[] _imageExt = { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };

        public MediaFactory()
        {
        }

        public IMedia createMedia(string pathName)
        {
            if (!this._allowedExt.Contains(Path.GetExtension(pathName)))
            {
                MessageBox.Show("File '" + Path.GetFileName(pathName) + "' could not be loaded!");
                return null;
            }

            if (this._videoExt.Contains(Path.GetExtension(pathName)))
                return new Video(pathName);
            else if (this._imageExt.Contains(Path.GetExtension(pathName)))
                return new mediaImage(pathName);
            return new Audio(pathName);
        }
    }
}
