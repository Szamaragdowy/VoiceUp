using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VoiceUP.Structures;
using VoiceUP.Network.TCP;
using VoiceUP.Windows;
using VoiceUP.Models;

namespace VoiceUP
{
    public partial class MainWindow : Window
    {
        private string _nickName = null;
        public MainWindow()
        {
            InitializeComponent();
            _nickName = ReadLastNickname();
            TextBoxLogin.Text = _nickName;
        }

        //edit view for specific server from list
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo ID = ((Button)sender).CommandParameter as ServerInfo;
            EditWindow nowe = new EditWindow(ID);
            nowe.Left = this.Left+8 ;
            nowe.Top = this.Top + 250;
            nowe.ShowDialog();

            int i = 0;
            foreach (var item in ComboBoxServerList.Items)
            {
               
                if(((ServerInfo)item).IP == ID.IP)
                {
                    string json = File.ReadAllText("MySerwers.txt");
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    jsonObj["MyServers"][i]["IP"] = nowe.IP;
                    jsonObj["MyServers"][i]["Port"] = nowe.PORT;
                    jsonObj["MyServers"][i]["Name"] = nowe.NAME;
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("MySerwers.txt", output);

                    ComboBoxServerList.ItemsSource = LoadMyServersFromJson();
                    break;
                }
                i++;
            } 

        }

        //deleting server from list
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo ID = ((Button)sender).CommandParameter as ServerInfo;

            if (MessageBox.Show("Czy na pewno chcesz usunąć ten serwer z listy?", "", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                string json = File.ReadAllText("MySerwers.txt");
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var jObj = (JObject)JsonConvert.DeserializeObject(json);
                var docsToRemove = new List<JToken>();

                foreach (var doc in jsonObj["MyServers"])
                {
                    var id = (string)doc["IP"];
                    if (ID.IP == id)
                    {
                        doc.Remove();
                        break;   
                    }
                }

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("MySerwers.txt", output);

                ComboBoxServerList.ItemsSource = LoadMyServersFromJson();
            }
        }

        //try to connect with server
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            
            bool valid = true;

            string login = TextBoxLogin.Text;
            string password = TextBoxPassword.Password;
            var selected = ComboBoxServerList.SelectedItem;
            SaveLastNickname(login);
            string ip = "";
            int port = 0;

            if (String.IsNullOrEmpty(login))
            {
                TextBoxLogin.BorderBrush = new SolidColorBrush(Colors.Red);
                WarningLogin.Visibility = Visibility.Visible;
                valid = false;
            }

            if (selected == null)
            {
                if (!String.IsNullOrEmpty(ComboBoxServerList.Text))
                {
                    Match match = MatchIP(ComboBoxServerList.Text);
                    if (match.Success)
                    {
                        ComboBoxServerList.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("89"), Convert.ToByte("000"), Convert.ToByte("000")));
                        WarningSerwer.Visibility = Visibility.Hidden;

                        string[] tab = match.Value.Split(':');

                        ip = tab[0];
                        port = Int32.Parse(tab[1]);
                    }
                    else
                    {
                        ComboBoxServerList.BorderBrush = new SolidColorBrush(Colors.Red);
                        WarningSerwer.Visibility = Visibility.Visible;
                        valid = false;
                    }
                }
            }
            else
            {
                var x = selected as ServerInfo;

                Match match = MatchIP(x.IP+":"+x.Port);
                if (match.Success)
                {
                    ComboBoxServerList.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("89"), Convert.ToByte("000"), Convert.ToByte("000")));
                    WarningSerwer.Visibility = Visibility.Hidden;

                    ip = x.IP;
                    port = Int32.Parse(x.Port);
                }
                else
                {
                    ComboBoxServerList.BorderBrush = new SolidColorBrush(Colors.Red);
                    WarningSerwer.Visibility = Visibility.Visible;
                    Console.WriteLine("No match.");
                    valid = false;
                }
            }

            if (valid)
            {
                TCPManager client = new TCPManager(ip, port);
                
                string connected = client.Connect(login, password);

                string[] data = connected.Split('/');
                switch (data[0])
                {
                    case "BAD_CHECKSUM":
                        Console.WriteLine("EROR-------WRONG CHECKSUM");
                        MessageBox.Show("Nie udało się zalogować do serwera.", "",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "FULL":
                        MessageBox.Show("Serwer jest pełen.", "",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case "LOGIN_ACK":
                        ServerWindow okno = new ServerWindow(client,data[1]);
                        okno.Left = this.Left;
                        okno.Top = this.Top;
                        this.Close();
                        okno.ShowDialog();

                        break;
                    case "LOGIN_NAK":
                        MessageBox.Show("Niepoprawne hasło.", "",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    default:
                        MessageBox.Show(connected, "Not Connected",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }
        }
        private string ReadLastNickname()
        {
            string name="";
            string json = File.ReadAllText("Settings.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            var jObj = (JObject)JsonConvert.DeserializeObject(json);
            foreach (var doc in jsonObj["Settings"])
            {
               name = (string)doc["NICKNAME"];
            }
            return name;
        }
        private void SaveLastNickname(string name)
        {
            string json = File.ReadAllText("Settings.txt");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["Settings"][0]["NICKNAME"] = name;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("Settings.txt", output);
        }

        #region LoadingData

        //loading data to combobox
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var Combo = sender as ComboBox;
            Combo.ItemsSource = LoadMyServersFromJson();
        }

        //loading data from json file
        public ObservableCollection<ServerInfo> LoadMyServersFromJson()
        {
            var collection = JsonLoader.LoadJson<MyServersJSON>("MySerwers.txt");

            if (collection!=null){
                return collection.MyServers;
            }

            return new ObservableCollection<ServerInfo>();   
        }

        //displaying in combobox
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedComboItem = sender as ComboBox;
            ServerInfo name = selectedComboItem.SelectedItem as ServerInfo;

            if (name != null)
            {
                selectedComboItem.Text = name.IP + ":" + name.Port;
            }
        }

        #endregion

        #region WindowBehavior

        //window Behavior on left click
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        #endregion

        #region Validation

        private Match MatchIP(string ip)
        {
            return Regex.Match(ip, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9]):[0-9]+$");
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WarningLogin == null) return;
            WarningLogin.Visibility = Visibility.Hidden;
            TextBoxLogin.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("89"), Convert.ToByte("000"), Convert.ToByte("000")));
        }

        private void ComboBoxServerList_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (WarningSerwer == null) return;
            WarningSerwer.Visibility = Visibility.Hidden;
            ComboBoxServerList.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("89"), Convert.ToByte("000"), Convert.ToByte("000")));
        }

        #endregion
    }
}