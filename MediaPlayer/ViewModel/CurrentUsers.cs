using MediaPlayer.Model;
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
        private User _loggedInUser = null;

        public CurrentUsers()
        {
            this._users = new MediaUsers();
            try
            {
                load();
            }
            catch
            {
                save();
            }
        }

        public CurrentUsers(CurrentUsers o)
        {
            this._users = o._users;
            try
            {
                load();
            }
            catch
            {
                save();
            }
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
                MessageBox.Show("Failed to save User!", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            }
        }

        public void load()
        {
            using (FileStream fs = new FileStream("Users.xml", FileMode.OpenOrCreate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MediaUsers));
                this._users = serializer.Deserialize(fs) as MediaUsers;
            }
        }

        public void addUser(string userName, string password)
        {
            if (userName.Count() < 4)
            {
                MessageBox.Show("Your UserName must be of at least 4 characters.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return;
            }
            if (password.Count() < 6)
            {
                MessageBox.Show("Your Password must be of at least 6 characters.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return;
            }
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, password);
                if (this._users.getUsers().Any(u => u.UserName == userName))
                {
                    MessageBox.Show("Error: UserName already exists.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                    return;
                }
                User user = new User(userName, hash);
                this._users.add(user);
                this.save();
                if (!Directory.Exists(PlaylistManager.PlaylistPath + userName))
                    Directory.CreateDirectory(PlaylistManager.PlaylistPath + userName);
                MessageBox.Show("Your account has been saved, please login.", "Login", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
            }
        }

        public void addUser(User user)
        {
            if (user.UserName.Count() < 4)
            {
                MessageBox.Show("Your UserName must be of at least 4 characters.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return;
            }
            if (this._users.getUsers().Any(u => u.UserName == user.UserName))
            {
                MessageBox.Show("Error: UserName already exists.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return;
            }
            this._users.add(user);
            this.save();
            if (!Directory.Exists(PlaylistManager.PlaylistPath + user.UserName))
                Directory.CreateDirectory(PlaylistManager.PlaylistPath + user.UserName);
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
            if (userName.Count() < 4)
            {
                MessageBox.Show("Your UserName must be of at least 4 characters.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return false;
            }
            if (password.Count() < 6)
            {
                MessageBox.Show("Your Password must be of at least 6 characters.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return false;
            }
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, password);
                if (!this._users.getUsers().Any(u => u.UserName ==  userName && u.Password == hash))
                {
                    MessageBox.Show("Error: UserName or password is incorrect.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                    return false;
                }
                User user = this._users.getUsers().Select(u => u).Where(u => u.UserName == userName && u.Password == hash).First();
                this._loggedInUser = user;
                return true;
            }
        }

        public bool checkUser(User user)
        {
            if (user.UserName.Count() < 4)
            {
                MessageBox.Show("Your UserName must be of at least 4 characters.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return false;
            }
            if (!this._users.getUsers().Contains(user))
                return false;
            this._loggedInUser = user;
            MessageBox.Show("Your have successfully logged in.", "Login", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
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

        public void changeUserName(string newUserName)
        {
            if (newUserName.Count() < 4)
            {
                MessageBox.Show("Your UserName must be of at least 4 characters.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return;
            }
            if (this._users.getUsers().Any(u => u.UserName == newUserName))
            {
                MessageBox.Show("Error: UserName already exists.", "Name Change Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                return;
            }
            User user = this._users.getUsers().Select(u => u).Where(u => u.UserName == this._loggedInUser.UserName).First();
            this._users.getUsers().Remove(user);
            if (MessageBox.Show("Would you like to keep your playlists?", "Name Change", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Directory.Move(PlaylistManager.PlaylistPath + user.UserName, PlaylistManager.PlaylistPath + newUserName);
            else
                Directory.Delete(PlaylistManager.PlaylistPath + user.UserName);
            user.UserName = newUserName;
            this.addUser(user);
            MessageBox.Show("Your UserName as been successfully changed.", "Name Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
        }

        public void changePassword(string oldPassword, string passwordOne, string passwordTwo)
        {
            if (oldPassword.Count() < 6 || passwordOne.Count() < 6 || passwordTwo.Count() < 6)
            {
                MessageBox.Show("Your password must be of at least 6 characters.", "Password Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return;
            }
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                string hashOld = GetMd5Hash(md5Hash, oldPassword);
                string hashOne = GetMd5Hash(md5Hash, passwordOne);
                string hashTwo = GetMd5Hash(md5Hash, passwordTwo);
                if (hashOne != hashTwo)
                {
                    MessageBox.Show("Password missmatch!", "Password Change", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.None);
                    return;
                }
                User user = this._users.getUsers().Select(u => u).Where(u => u.UserName == this._loggedInUser.UserName).First();
                if (user.Password != hashOld)
                {
                    MessageBox.Show("Your password is incorrect !", "Password Change", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.None);
                    return;
                }
                this._users.getUsers().Remove(user);
                user.Password = hashOne;
                this.addUser(user);
                MessageBox.Show("Your Password as been successfully changed.", "Password Change", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
            }
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
