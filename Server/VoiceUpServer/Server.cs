using VoiceUpServer.Models;
using VoiceUpServer.Structures;

namespace VoiceUpServer
{
    class Server
    {
        private string Name;

        public string ServerName
        {
            get { return Name; }
            set { Name = value; }
        }


        UsersList usersList { get; set; }

        public Server(string Name, string ip, int port,int maxusers)
        {
            this.Name = Name;
            this.usersList = new UsersList();
        }

        //aktualna lista użytkowników na serwerze
        public UsersList ReturnActualListOfUsers => usersList;

        //wyciszenie/odciszenie kogoś mikrofonu
        public void ChangeUserMicrophoneStatus(User user)
        {
            usersList.ChangeUserMicrophoneStatus(user);
        }

        //przesyłanie / nie przesyłanie dzwięku do danego uzytkownika
        public void ChangeUserSoundStatus(User user)
        {
            usersList.ChangeUserSoundStatus(user);
        }

        //wyrzucenie użytkownika
        public void KickUser(User user)
        {
            usersList.KickUser(user);
        }

        //dodaje użytkownika do listy - wywołać po poprawnym uwierzytelnieniu
        public void AddNewUser(User user)
        {
            usersList.AddUser(user);
        }
    }
}
