using System.Collections.ObjectModel;
using System.Threading;
using VoiceUpServer.Models;

namespace VoiceUpServer
{
    class Server
    {

        private ObservableCollection<User> usersList;

        public ObservableCollection<User> ActualListOfUsers => usersList;

        private string Name;

        public string ServerName
        {
            get { return Name; }
            set { Name = value; }
        }
       
        public Server(string Name, string ip, int port,int maxusers)
        {
            this.Name = Name;
            this.usersList = new ObservableCollection<User>();
        }

        public void start()
        {
            load();//do usuniecie potem
        }

        public void stop()
        {
            clearList();
        }

        private void clearList()
        {
            usersList.Clear();
        }

        //to tylko dla TESTow
        private void load()
        {
            usersList.Add(new User("Willa"));
            usersList.Add(new User("Isak"));
            usersList.Add(new User("Victor"));
            usersList.Add(new User("Jules"));
        }

        //wyciszenie/odciszenie kogoś mikrofonu
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

        //przesyłanie / nie przesyłanie dzwięku do danego uzytkownika
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
    }
}
