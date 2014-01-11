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
        private string[] _allowedExt = { ".mp3", ".mp4", ".asf", ".3gp", ".3g2", ".asx", ".avi", ".jpg", ".jpeg", ".gif", ".bmp", ".png" };
        private string[] _videoExt = { ".mp4", ".asf", ".3gp", ".3g2", ".asx", ".avi"};
        private string[] _imageExt = { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };

        public MediaFactory()
        {
        }

        public Media createMedia(string pathName)
        {
            if (!this._allowedExt.Contains(System.IO.Path.GetExtension(pathName)))
            {
                Console.Out.WriteLine("Error tmp");
            }

            Media.Type type = Media.Type.AUDIO;
            if (this._videoExt.Contains(System.IO.Path.GetExtension(pathName)))
                type = Media.Type.VIDEO;
            else if (this._imageExt.Contains(System.IO.Path.GetExtension(pathName)))
                type = Media.Type.IMAGE;
            Media media = new Media(69, "n/a", pathName, type);
            return media;
        }
    }
}
