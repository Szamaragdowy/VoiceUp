using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VoiceUP.Interfaces;

namespace VoiceUP.Models
{
    public class ServerInfo : IServerInfo, INotifyPropertyChanged
    {
        public string IP { get; set; }

        public string Port { get; set; }

        public string Name { get; set; }

        public ServerInfo(string ip,string port, string name)
        {
            this.IP = ip;
            this.Port = port;
            this.Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return IP + ":" + Port;
        }
    }
}
