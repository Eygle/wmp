﻿using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using MediaPlayer;

namespace MediaPlayer.ViewModel
{
    class CurrentUsers
    {
        private MediaUsers _users;
        private User _loggedInUser;

        public CurrentUsers()
        {
            this._users = new MediaUsers();
        }

        public CurrentUsers(CurrentUsers o)
        {
            this._users = o._users;
        }

        public void save()
        {
            try
            {
                using (FileStream fs = new FileStream("Users.xml", FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MediaUsers));
                    serializer.Serialize(fs, this._users);
                }
            }
            catch
            {
                MessageBox.Show("Failed to save User!");
            }
        }

        public void load()
        {
            using (FileStream fs = new FileStream("Users.xml", FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MediaUsers));
                this._users = serializer.Deserialize(fs) as MediaUsers;
            }
        }

        public void addUser(string userName, string password)
        {
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, password);
                User user = new User(userName, password);
                this._users.add(user);
            }
        }

        public void addUser(User user)
        {
            this._users.add(user);
        }

        public void removeUser(string userName, string password)
        {
            User user = new User(userName, password);
            this._users.remove(user);
        }

        public void removeUser(User user)
        {
            this._users.remove(user);
        }

        public bool checkUser(string userName, string password)
        {
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, password);
                User user = new User(userName, password);
                if (!this._users.getUsers().Contains(user))
                    return false;
                this._loggedInUser = user;
                return true;
            }
        }

        public bool checkUser(User user)
        {
            if (!this._users.getUsers().Contains(user))
                return false;
            this._loggedInUser = user;
            return true;
        }

        public User getLoggedUser()
        {
            return this._loggedInUser;
        }

        public void logoutUser()
        {
            this._loggedInUser = null;
        }

        private static string GetMd5Hash(System.Security.Cryptography.MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
