using System.Collections.ObjectModel;
using System.Threading;
using VoiceUpServer.Models;

namespace VoiceUpServer
{
    class Server
    {
        private ObservableCollection<User> usersList;
        private string _ServerName;
        private string _ServerIP;
        private int _ServerPORT;
        private int _MaxUsers;

        #region propertasy
        public ObservableCollection<User> ActualListOfUsers => usersList;
        public string ServerName
        {
            get { return _ServerName; }
            set { _ServerName = value; }
        }
        public string ServerIP
        {
            get { return _ServerIP; }
            set { _ServerIP = value; }
        }
        public int ServerPort
        {
            get { return _ServerPORT; }
            set { _ServerPORT = value; }
        }
        public int MaxUsers
        {
            get { return _MaxUsers; }
            set { _MaxUsers = value; }
        }
        #endregion

        public Server(string Name, string ip, int port,int maxusers)
        {
            this._ServerName = Name;
            this._ServerIP = ip;
            this._ServerPORT = port;
            this._MaxUsers = maxusers;
            this.usersList = new ObservableCollection<User>();
        }

        public void start()
        {
            //tutaj trzeba włączyć jakieś nasłuchiwanie na porcie itp


            AddNewUser(new User("Willa","192.168.1.150"));
        }

        public void stop()
        {
            clearList();
        }

        //wyrzuca wszystkich uzytkowników z listy
        private void clearList()
        {
            usersList.Clear();
        }

        //wyrzucenie użytkownika
        public void KickUser(User user)
        {
            usersList.Remove(user);
        }

        //dodaje użytkownika do listy - wywołać po poprawnym uwierzytelnieniu
        public void AddNewUser(User user)
        {
            usersList.Add(user);
        }


        #region servergui
        //wyciszenie/odciszenie  - podpiętę do przycisków gui
        public void ChangeUserMicrophoneStatus(User user)
        {
            if (!user.Mute)
            {
                user.Mute = true;
            }
            else
            {
                user.Mute = false;
            }
        }

        //przesyłanie /nie przesyłanie dzwięku -  podpiętę do przycisków gui
        public void ChangeUserSoundStatus(User user)
        {
            if (!user.SoundOff)
            {
                user.SoundOff = true;
            }
            else
            {
                user.SoundOff = false;
            }
        }
        #endregion
    }
}
