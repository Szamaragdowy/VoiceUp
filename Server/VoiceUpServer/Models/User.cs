using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUpServer.Models
{
    public class User : INotifyPropertyChanged
    {
        private bool _IsMuted;
        private bool _IsSoundOff;
        private string _IP;

        public string Name { get; set; }

        public string IP
        {
            get { return _IP; }
        }
        
        public User()
        {

        }

        public User(string name, string ip)
        { 
            this.Name = name;
            this._IP = ip;
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
