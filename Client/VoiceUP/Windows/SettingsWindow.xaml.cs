using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VoiceUP.Models;
using VoiceUP.Structures;

namespace VoiceUP.Windows
{
    public partial class SettingsWindow : Window
    {
        private SoundManager soundManager;
        private int selectedDeviceInedx = -1;
        private string _serverIpPort;
        private string _serverName;
        public SettingsWindow(SoundManager sm, string serverIpPort, string serverName)
        {
            InitializeComponent();
            this.soundManager = sm;
            this._serverIpPort = serverIpPort;
            this._serverName = serverName;
        }
        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.ItemsSource = soundManager.ListOfMicrophones();
        }
        private void SoundDeviceBoxLoaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.ItemsSource = soundManager.ListOfSoundDevice();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            selectedDeviceInedx = combo.SelectedIndex;
            soundManager.setMicrophoneIndex(selectedDeviceInedx);
        }
        private void SoundDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            selectedDeviceInedx = combo.SelectedIndex;
            soundManager.setSoundDeviceIndex(selectedDeviceInedx);
        }
        private void ButtonStartTest_Click(object sender, RoutedEventArgs e)
        {
            ClearLabelInfo();
            if (soundManager.StartREcording())
            {
                SetLabelInfo("Powiedz coś :)", new SolidColorBrush(Colors.Green));
                ButtonStopTest.IsEnabled = true;
                ButtonStartTest.IsEnabled = false;
            }
            else
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
        private void AddBookMark_Click(object sender, RoutedEventArgs e)
        {
            bool check = false;
            string temp;
            string json = File.ReadAllText("MySerwers.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var IpPort = _serverIpPort.Split(':');
            var jObj = (JObject)JsonConvert.DeserializeObject(json);
            foreach (var doc in jsonObj["MyServers"])
            {
                var ip = (string)doc["IP"];
                var port = (string)doc["PORT"];
                if (IpPort[0] == ip||IpPort[1] == port)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                temp = output.Substring(0, output.Length - 7);
                string newSerwer = ",    \n    {\r\n      \"IP\": \"" + IpPort[0]+ "\",\r\n      \"Port\": \"" + IpPort[1]+ "\",\r\n      \"Name\": \"" + this._serverName+ "\"\r\n    }\r\n  ]\r\n}";
                temp += newSerwer;
                File.WriteAllText("MySerwers.txt", temp);
                MessageBox.Show("Dodano do Twoich serwerów");
            }
            else
            {
                MessageBox.Show("Ten serwer już należy do Twoich serwerów");
            }
        }
    }
}
