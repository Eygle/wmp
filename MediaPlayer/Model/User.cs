using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MediaPlayer.Model
{
    public class User
    {
        private string _userName;
        private string _password;

        public User()
        {
        }

        public User(string userName, string password)
        {
            this._userName = userName;
            this._password = password;
        }

        public string UserName
        {
            get { return this._userName; }
            set { this._userName = value; }
        }

        public string Password
        {
            get { return this._password; }
            set { this._password = value; }
        }
    }
}
