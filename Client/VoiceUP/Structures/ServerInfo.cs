using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VoiceUP.Interfaces;

namespace VoiceUP.Structures
{
    public class ServerInfo : IServerInfo, INotifyPropertyChanged
    {
        public IPEndPoint IPAdres { get; }

        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerInfo(IPEndPoint ip, String name)
        {
            this.IPAdres = ip;
            this.Name = name;
        }

        public override string ToString()
        {
            return IPAdres.ToString();
        }

        public void Edit(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Delete(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
