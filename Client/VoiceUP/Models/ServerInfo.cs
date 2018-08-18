using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VoiceUP.Models
{
    //model to storing info about voice servers
    public class ServerInfo :  INotifyPropertyChanged
    {
        public ServerInfo(string ip, string port, string name)
        {
            this.IP = ip;
            this.Port = port;
            this.Name = name;
        }

        #region properties

        public string IP { get; set; }

        public string Port { get; set; }

        //Name of server, received from server after properly connection 
        //Can be also changed by user in EditWindow 
        public string Name { get; set; }

        #endregion

        #region INotify

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        //using for display e.g. in combobox
        public override string ToString()
        {
            return IP + ":" + Port;
        }
    }
}
