using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VoiceUP.Structures;

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

        public ObservableCollection<UserInfo> collection { get; set; }

        public ServerWindow()
        {
            InitializeComponent();
            collection = new ObservableCollection<UserInfo>();
            _isMuted = false;
            _isSoundOf = false;
            _soundManager = new SoundManager();
        }

        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.ItemsSource = collection;
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

        }
    }
}
