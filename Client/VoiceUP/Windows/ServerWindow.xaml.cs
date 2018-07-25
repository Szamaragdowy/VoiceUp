using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VoiceUP.Structures;
using VoiceUP.TCP;

namespace VoiceUP.Windows
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        private SoundManager _soundManager;
        private bool _isMuted;
        private bool _isSoundOf;
        private myTCPClient _Tcpclient;

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

        public ObservableCollection<UserInfo> _collection { get; set; }

        public ServerWindow(myTCPClient client)
        {
            InitializeComponent();
            this._isMuted = false;
            this._isSoundOf = false;
            this._soundManager = new SoundManager();
            this._Tcpclient = client;
            this._collection = _Tcpclient.GetCurrentUserList();


            //_collection.Add(new UserInfo("krokodyl"));
        }

        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.ItemsSource = _collection;
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
            okno.ShowDialog();

        }


        //rozłączenie z serwerem
        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            //AsynchronousClient.send("CYA/<EOF>")
            //   _TCPConnection.SendMessageToServer("CYA/<EOF>");
        }
    }
}
