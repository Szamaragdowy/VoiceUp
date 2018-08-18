using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VoiceUP.Structures;
using VoiceUP.TCP;
using VoiceUP.Models;

namespace VoiceUP.Windows
{
    public partial class ServerWindow : Window
    {
        public ObservableCollection<UserInfo> _collection { get; set; }
        private SoundManager _soundManager;
        private bool _isMuted;
        private bool _isSoundOf;
        private myTCPClient _Tcpclient;
        private string _serverName;

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

        public ServerWindow(myTCPClient client,string ServerName)
        {
            InitializeComponent();
            this._isMuted = false;
            this._isSoundOf = false;
            this._soundManager = new SoundManager();
            this._Tcpclient = client;
            this._Tcpclient.setDeleagats(kicked, ServerBye);
            this._Tcpclient.startUDP(_soundManager.microphoneIndex);
            labelServerName.Content = ServerName;
            labelIpPort.Content = this._Tcpclient.GetIPAndPort();
        }

        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.ItemsSource = this._Tcpclient.getList();
        }

        //wyciszanie mikrofonu
        private void ButtonMic_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonMic.Content == FindResource("Mic_On"))
            {
                ButtonMic.Content = FindResource("Mic_Off");
                Mute();     
            }
            else
            {
                ButtonMic.Content = FindResource("Mic_On");
                UnMute();
            }
        }

        //wyciszanie dźwięku
        private void ButtonSound_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonSound.Content == FindResource("Sound_On"))
            {
                ButtonSound.Content = FindResource("Sound_Off");
                SoundOff();
            }
            else
            {
                ButtonSound.Content = FindResource("Sound_On");
                SoundOn();
            }
        }

        //przejscie do ustawień
        private void Buttonsetting_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow okno = new SettingsWindow(_soundManager);
            okno.Left = this.Left;
            okno.Top = this.Top;
            okno.ShowDialog();
            _Tcpclient.maybeMicrophoneChanged(_soundManager);
        }

        //rozłączenie z serwerem
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
