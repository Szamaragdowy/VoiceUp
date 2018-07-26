using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUpServer.Models
{
    public class User : INotifyPropertyChanged
    {
        public Socket workSocket { get; set; }
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        private bool _IsMuted;
        private bool _IsSoundOff;

        public IPEndPoint _ipendpoint;

        public string Name { get; set; }

       
        public User()
        {

        }

        public User(Socket socket,int udp_port)
        { 
            this.workSocket = socket;
            _ipendpoint = new IPEndPoint((((IPEndPoint)socket.RemoteEndPoint).Address), udp_port);
        }

        public bool Mute
        {
            get { return _IsMuted; }
            set
            {
                _IsMuted = value;
                OnPropertyChange("Mute");
            }
        }

        public bool SoundOff
        {
            get { return _IsSoundOff; }
            set
            {
                _IsSoundOff = value;
                OnPropertyChange("SoundOff");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChange(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
