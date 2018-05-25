using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceUpServer.Models;

namespace VoiceUpServer
{
    class Server
    {
        ObservableCollection<User> collection = new ObservableCollection<User>();

        public Server()
        {
            LoadExample();
        }

        private void LoadExample()
        {
            collection.Add(new User("Marek"));
            collection.Add(new User("Sławomir"));
            collection.Add(new User("Ola"));
            collection.Add(new User("Zbyszek"));
            collection.Add(new User("Pioter"));
            collection.Add(new User("Mandaryna"));
            collection.Add(new User("Wojtek"));
            collection.Add(new User("Krzysztof"));
        }

        public ObservableCollection<User> ReturnList()
        {
            return collection;
        }

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

        public void KickUser(User user)
        {
            collection.Remove(user);
        }
    }
}
