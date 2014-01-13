using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MediaPlayer.Model
{
    [Serializable]
    public class MediaUsers
    {
        [XmlElement(ElementName = "Content")]
        private List<User> _content;

        public MediaUsers()
        {
            this._content = new List<User>();
        }

        public MediaUsers(List<User> content)
        {
            this._content = content;
        }

        public void add(User user)
        {
            this._content.Add(user);
        }

        public void remove(User user)
        {
            this._content.Remove(user);
        }

        public List<User> getUsers()
        {
            return this._content;
        }
    }
}
