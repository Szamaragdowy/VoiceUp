using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class SettingsWindow : Window
    {

        private SoundManager soundManager = new SoundManager();

        private int seleccteDeviceInedx =-1;

        public SettingsWindow()
        {
            InitializeComponent();
        }


        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {

            var combo = sender as ComboBox;
            combo.ItemsSource = soundManager.ListOfMicrophones();
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            seleccteDeviceInedx = combo.SelectedIndex;
            soundManager.setMicrophoneIndex(seleccteDeviceInedx);
        }

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

        private void SetLabelInfo(string msg, SolidColorBrush color)
        {
            LabelInfo.Content = msg;
            LabelInfo.Foreground = color;
            
        }

        private void ClearLabelInfo()
        {
            LabelInfo.Content = "";
            LabelInfo.Foreground = new SolidColorBrush(Colors.Black);
        }


    }
}
