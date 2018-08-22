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
        private int selectedMicrophoneIndex;
        private int selectedSoundDeviceIndex;
        private string _serverIpPort;
        private string _serverName;
        public SettingsWindow(SoundManager sm, string serverIpPort, string serverName)
        {
            InitializeComponent();
            ReadSettings();
            this.soundManager = sm;
            this._serverIpPort = serverIpPort;
            this._serverName = serverName;
            CheckSettings();
        }
        private void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.ItemsSource = soundManager.ListOfMicrophones();
            combo.SelectedIndex = selectedMicrophoneIndex;
        }
        private void SoundDeviceBoxLoaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.ItemsSource = soundManager.ListOfSoundDevice();
            combo.SelectedIndex = selectedSoundDeviceIndex;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            selectedMicrophoneIndex = combo.SelectedIndex;
            soundManager.setMicrophoneIndex(selectedMicrophoneIndex);
            CheckSettings();
        }
        private void SoundDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            selectedSoundDeviceIndex = combo.SelectedIndex;
            soundManager.setSoundDeviceIndex(selectedSoundDeviceIndex);
            CheckSettings();
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
        //checking that serwer is on our MyServer list
        public void Check()
        {
            bool isInFavorities = true;
            string json = File.ReadAllText("MySerwers.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var IpPort = _serverIpPort.Split(':');
            var jObj = (JObject)JsonConvert.DeserializeObject(json);
            foreach (var doc in jsonObj["MyServers"])
            {
                var ip = (string)doc["IP"];
                var port = (string)doc["PORT"];
                if (IpPort[0] == ip || IpPort[1] == port)
                {
                    isInFavorities = false;
                    break;
                }
            }
            if (!isInFavorities) ButtonFavorite.IsEnabled = false;
            else ButtonFavorite.IsEnabled = true;
            
        }
        private void AddBookMark_Click(object sender, RoutedEventArgs e)
        {
            string temp;
            string json = File.ReadAllText("MySerwers.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var IpPort = _serverIpPort.Split(':');
            var jObj = (JObject)JsonConvert.DeserializeObject(json);
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            temp = output.Substring(0, output.Length - 7);
            string newSerwer = ",    \n    {\r\n      \"IP\": \"" + IpPort[0] + "\",\r\n      \"Port\": \"" + IpPort[1] + "\",\r\n      \"Name\": \"" + this._serverName + "\"\r\n    }\r\n  ]\r\n}";
            temp += newSerwer;
            File.WriteAllText("MySerwers.txt", temp);
            MessageBox.Show("Dodano do ulubionych");
            Check();
        }
        public void ReadSettings()
        {
            string json = File.ReadAllText("Settings.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var jObj = (JObject)JsonConvert.DeserializeObject(json);
            foreach (var doc in jsonObj["Settings"])
            {
                selectedMicrophoneIndex = doc["MICROPHONE"];
                selectedSoundDeviceIndex = doc["SOUND"];
            }
        }
        public void CheckSettings()
        {
            bool isInSettings = false;
            string json = File.ReadAllText("Settings.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var jObj = (JObject)JsonConvert.DeserializeObject(json);
            foreach (var doc in jsonObj["Settings"])
            {
                var sound = (string)doc["SOUND"];
                var micro = (string)doc["MICROPHONE"];
                if (sound != selectedSoundDeviceIndex.ToString() || micro != selectedMicrophoneIndex.ToString())
                {
                    isInSettings = true;
                    break;
                }
            }
            if (isInSettings) ButtonSave.IsEnabled = true;
            else ButtonSave.IsEnabled = false;
        }
        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            string json = File.ReadAllText("Settings.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["Settings"][0]["SOUND"] = selectedSoundDeviceIndex;
            jsonObj["Settings"][0]["MICROPHONE"] = selectedMicrophoneIndex;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("Settings.txt", output);
            MessageBox.Show("Zapisano zmiany");
            CheckSettings();
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
