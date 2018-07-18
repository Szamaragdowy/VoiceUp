using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceUpServer.Models;
using System.Net.Sockets;
using System.Net;


namespace VoiceUpServer
{
    public class ReceivedDataArgs
    {
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
        public byte[] ReceivedBytes;

        public ReceivedDataArgs(IPAddress ip , int port, byte[] data)
        {
            this.IPAddress = ip;
            this.Port = port;
            this.ReceivedBytes = data;
        }
    }


    class Server
    {
        ObservableCollection<User> collection = new ObservableCollection<User>();

        public void Listen()
        {
            UdpClient listener = new UdpClient(5000);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 5000);

            while(true)
            {

                byte[] data = listener.Receive(ref serverEP);
                RaiseDataReceived(new ReceivedDataArgs(serverEP.Address,
                    serverEP.Port, data));
                //raise event
            }
        }

        public delegate void DataReceived(object sender, ReceivedDataArgs args );

        public event DataReceived DataREceivedEvent;

        private void RaiseDataReceived(ReceivedDataArgs args)
        {
            if (DataREceivedEvent != null)
                DataREceivedEvent(this, args);
        }


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
