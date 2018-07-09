using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoiceUP.Structures;

namespace VoiceUP
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {

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

            createUsers();
        }

        private void createUsers()
        { 
            collection.Add(new UserInfo("Marek"));
            collection.Add(new UserInfo("Sławomir"));
            collection.Add(new UserInfo("Ola"));
            collection.Add(new UserInfo("Zbyszek"));
            collection.Add(new UserInfo("Pioter"));
            collection.Add(new UserInfo("Mandaryna"));
            collection.Add(new UserInfo("Wojtek"));
            collection.Add(new UserInfo("Krzysztof"));

        }

        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {

            var listbox = sender as ListBox;
            listbox.ItemsSource = collection;
        }

        private void ButtonMic_Click(object sender, RoutedEventArgs e)
        {
            
            
            if (ButtonMic.Content == FindResource("Mic_On"))
            {
                ButtonMic.Content = FindResource("Mic_Off");
                Mute();
                
               // var but = sender as Button;
                //var bc = new BrushConverter();
                //but.Background = (Brush)bc.ConvertFrom("#FFXXXXXX");
            }
            else
            {
                ButtonMic.Content = FindResource("Mic_On");
                UnMute();
            }
        }

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

        private void Buttonsetting_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow okno = new SettingsWindow();
            bool connected = true;
            if (connected)
            {
               // this.Close();
                okno.ShowDialog();
            }
            else
            {
                MessageBox.Show("PUPA, nie połączyłeś się :/", "",
                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
