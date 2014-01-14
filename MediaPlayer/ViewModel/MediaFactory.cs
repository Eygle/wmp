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
        private delegate IMedia CreateMediaDelegate(string pathname);

        private Dictionary<string, CreateMediaDelegate> _allowedExt = new Dictionary<string, CreateMediaDelegate>()
        { 
            {".mp3", new CreateMediaDelegate(createAudio) },
            {".wma", new CreateMediaDelegate(createAudio) },
            {".mov", new CreateMediaDelegate(createVideo) },
            {".avi", new CreateMediaDelegate(createVideo) },
            {".wmv", new CreateMediaDelegate(createVideo) },
            {".png", new CreateMediaDelegate(createImage) },
            {".jpg", new CreateMediaDelegate(createImage) },
            {".jpeg", new CreateMediaDelegate(createImage) },
            {".bmp", new CreateMediaDelegate(createImage) },
            {".gif", new CreateMediaDelegate(createImage) }
        };

        public MediaFactory()
        {
        }

        private static IMedia createVideo(string pathname)
        {
            return new Video(pathname);
        }

        private static IMedia createAudio(string pathname)
        {
            return new Audio(pathname);
        }

        private static IMedia createImage(string pathname)
        {
            return new Image(pathname);
        }

        public IMedia createMedia(string pathName)
        {
            try
            {
                if (this._allowedExt.ContainsKey(Path.GetExtension(pathName)))
                    return this._allowedExt[Path.GetExtension(pathName)](pathName);
                MessageBox.Show("File '" + Path.GetFileName(pathName) + "' could not be loaded!");
                return null;
            }
            catch
            {
                MessageBox.Show("File '" + Path.GetFileName(pathName) + "' could not be loaded!");
                return null;
            }
        }
    }
}
