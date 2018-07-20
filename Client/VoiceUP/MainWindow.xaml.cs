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
using VoiceUP.UDP;
using VoiceUP.Windows;

namespace VoiceUP
{
    public partial class MainWindow : Window
    {
        //konstruktor
        public MainWindow()
        {
            InitializeComponent();
        }

        //przejsćie do edycji wybranego serwera z listy
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo ID = ((Button)sender).CommandParameter as ServerInfo;
            EditWindow nowe = new EditWindow(ID);
            nowe.Left = this.Left ;
            nowe.Top = this.Top + 205;
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

        //usuwanie wybranego serwera z listy
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

        //próba połaczenia z serwerem.
        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            string login = TextBoxLogin.Text;
            string password = TextBoxPassword.Password;
            var selected = ComboBoxServerList.SelectedItem;

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
                //tu i tak będzie TCP połączanie.
                ConnectWithServerUDP serverUDP = new ConnectWithServerUDP(ip, port);

                bool connected = serverUDP.Connect();
                if (connected)
                {
                    ServerWindow okno = new ServerWindow();
                    this.Close();
                    okno.ShowDialog();
                }
                else
                {
                    MessageBox.Show("PUPA, nie połączyłeś się :/", "",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //wyjście z aplikacji
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        #region LoadingData

        //ładowanie wczytanych serwerów do comboboxa
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var Combo = sender as ComboBox;
            Combo.ItemsSource = LoadMyServersFromJson();
        }

        //wczytuje liste serwerów z plików json
        public ObservableCollection<ServerInfo> LoadMyServersFromJson()
        {
            return JsonLoader.LoadJson<MyServersJSON>("MySerwers.txt").MyServers;
        }

        //wyświetlenie odpowiedniego ip po zmianie wyboru w comboboxie
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

        //poruszanie oknem przytrzymując lewy przycisk myszy
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        #endregion

        #region Validation

        //regex do ip format (IP:port) - ip 4 x mas 255 i port zawierający co najmniej jedną liczbę
        private Match MatchIP(string ip)
        {
            return Regex.Match(ip, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9]):[0-9]+$");
        }

        //po zmianie textu w loginie, zmiana na domyślny kolor
        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            WarningLogin.Visibility = Visibility.Hidden;
            TextBoxLogin.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("89"), Convert.ToByte("000"), Convert.ToByte("000")));
        }

        //po zmianie textu w wyborze serwera, zmiana na domyślny kolor
        private void ComboBoxServerList_TextInput(object sender, TextCompositionEventArgs e)
        {
            WarningSerwer.Visibility = Visibility.Hidden;
            ComboBoxServerList.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("89"), Convert.ToByte("000"), Convert.ToByte("000")));
        }

        #endregion
    }
}