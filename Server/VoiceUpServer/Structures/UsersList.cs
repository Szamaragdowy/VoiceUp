using System.Collections.ObjectModel;
using VoiceUpServer.Models;

namespace VoiceUpServer.Structures
{
    public class UsersList : ObservableCollection<User>
    {
        public UsersList() : base()
        {
            Add(new User("Willa"));
            Add(new User("Isak"));
            Add(new User("Victor"));
            Add(new User("Jules"));
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
            Remove(user);
        }

        public void AddUser(User user)
        {
            Add(user);
        }
    }
}