using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VoiceUP.Structures;

namespace VoiceUP.Windows
{
    public partial class SettingsWindow : Window
    {
        private SoundManager soundManager; //referencja do menadżera dzwięku

        private int selectedDeviceInedx =-1; //zmiena z indexem wybranego mikrofonu

        //konstruktorek
        public SettingsWindow(SoundManager sm)
        {
            InitializeComponent();
            this.soundManager = sm;
        }

        //załadowanie listy dostępnych mikrofonów
        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.ItemsSource = soundManager.ListOfMicrophones();
        }

        //zmiana mikrofonu po wybraniu z listy
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            selectedDeviceInedx = combo.SelectedIndex;
            soundManager.setMicrophoneIndex(selectedDeviceInedx);
        }

        //włączenie testu dzwięku
        private void ButtonStartTest_Click(object sender, RoutedEventArgs e)
        {
            ClearLabelInfo();
            if (soundManager.StartREcording())
            {
                SetLabelInfo("Powiedz coś :)", new SolidColorBrush(Colors.Green));
                ButtonStopTest.IsEnabled = true;
                ButtonStartTest.IsEnabled = false;
            }else
            {
                SetLabelInfo("Nie wybrano urządzenia do nagrywania", new SolidColorBrush(Colors.Red));
            }
           
        }

        //zatrzymanie testu dzwięku
        private void ButtonStopTest_Click(object sender, RoutedEventArgs e)
        {
            ClearLabelInfo();
            if (soundManager.StopRecording())
            {    
                ButtonStopTest.IsEnabled = false;
                ButtonStartTest.IsEnabled = true;
            }
            else
            {
                SetLabelInfo("Występił błąd podczas zatrzymywania testu", new SolidColorBrush(Colors.Red));
            }
        }

        //wyświtlanie komunikatów w ustawieniach
        private void SetLabelInfo(string msg, SolidColorBrush color)
        {
            LabelInfo.Content = msg;
            LabelInfo.Foreground = color;     
        }

        //czyszczenie komunikatu w ustawieniach
        private void ClearLabelInfo()
        {
            LabelInfo.Content = "";
            LabelInfo.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}
