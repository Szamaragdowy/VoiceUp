using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VoiceUP.Structures;
using VoiceUP.Network.TCP;
using VoiceUP.Network.UDP;
using VoiceUP.Models;

namespace VoiceUP.Windows
{
    public partial class ServerWindow : Window
    {
        public ObservableCollection<UserInfo> _collection { get; set; }
        private SoundManager _soundManager;
        private bool _isMuted;
        private bool _isSoundOf;
        private TCPManager _Tcpclient;
        private string _serverName;
        private string _serverIpPort;

        #region helpers
        #region microphone
        public void Mute()
        {
            _isMuted = true; 
        }

        public void UnMute()
        {
            _isMuted = false;
        }
        #endregion
        #region sound
        public void SoundOn()
        {
            _isSoundOf = false;
        }

        public void SoundOff()
        {
            _isSoundOf = true;
        }
        #endregion
        #endregion

        public ServerWindow(TCPManager client,string ServerName)
        {
            InitializeComponent();
            this._isMuted = false;
            this._isSoundOf = false;
            this._soundManager = new SoundManager();
            this._Tcpclient = client;
            this._Tcpclient.setDeleagats(kicked, ServerBye, ServerMuted, ServerUnMuted,ServerSoundOff,ServerUnSoundOff);
            this._Tcpclient.startUDP(_soundManager.microphoneIndex);
            labelServerName.Content = ServerName;
            _serverName = ServerName;
            this.Title = "VoiceUp (" + ServerName + ")";
            labelIpPort.Content = this._Tcpclient.GetIPAndPort();
            _serverIpPort = this._Tcpclient.GetIPAndPort();
        }
        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.ItemsSource = this._Tcpclient.getList();
        }

        private void ButtonMic_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonMic.Content == FindResource("Mic_On"))
            {
                ButtonMic.Content = FindResource("Mic_Off");
                _Tcpclient.Mute();
            }
            else
            {
                ButtonMic.Content = FindResource("Mic_On");
                _Tcpclient.unMute();
            }
        }

        private void ButtonSound_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonSound.Content == FindResource("Sound_On"))
            {
                ButtonSound.Content = FindResource("Sound_Off");
                _Tcpclient.SoundOff();
            }
            else
            {
                ButtonSound.Content = FindResource("Sound_On");
                _Tcpclient.unSoundOff();
            }
        }

        private void Buttonsetting_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow okno = new SettingsWindow(_soundManager,this._serverIpPort,this._serverName);
            okno.Left = this.Left;
            okno.Top = this.Top;
            okno.ShowDialog();
            _Tcpclient.maybeMicrophoneChanged(_soundManager);
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            _Tcpclient.Discconect();
            MainWindow okno = new MainWindow();
            okno.Left = this.Left;
            okno.Top = this.Top;
            this.Close();
            okno.ShowDialog();
        }

        public bool kicked()
        {
            try
            {
                _Tcpclient.closeAfterDisconect();
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageBox.Show("Zostałeś wyrzucony.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow okno = new MainWindow();
                    okno.Left = this.Left;
                    okno.Top = this.Top;
                    okno.ShowDialog();
                });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool ServerMuted()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageBox.Show("Zostało Ci zabrane prawo do mówienia.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool ServerUnMuted()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageBox.Show("Zostało Ci przywrócone prawo do mówienia.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool ServerSoundOff()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageBox.Show("Serwer zablokował Ci możliwość słuchania konwersacji", "", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool ServerUnSoundOff()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageBox.Show("Serwer przywrócił Ci możliwość słuchania konwersacji", "", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public bool ServerBye()
        {
            try
            {
                _Tcpclient.closeAfterDisconect();
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageBox.Show("Serwer został wyłączony.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow okno = new MainWindow();
                    okno.Left = this.Left;
                    okno.Top = this.Top;
                    this.Close();
                    okno.ShowDialog();
                });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
