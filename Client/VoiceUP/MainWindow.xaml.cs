using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceUP.Structures;

namespace VoiceUP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ServerInfo> collection { get; set; }

        public MainWindow()
        {
            InitializeComponent();

        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var Combo = sender as ComboBox;
            Combo.ItemsSource = LoadJson<MyServersJSON>("MySerwers.txt").MyServers;
        }

        public void LoadComboBoxItems()
        {
            ComboBoxServerList.ItemsSource = LoadJson<MyServersJSON>("MySerwers.txt").MyServers;
        }

        public T LoadJson<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
         }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedComboItem = sender as ComboBox;
            ServerInfo name = selectedComboItem.SelectedItem as ServerInfo ;


            if (name != null)
            {
                selectedComboItem.Text = name.IP +":"+name.Port;
            }

        }


        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo ID = ((Button)sender).CommandParameter as ServerInfo;
            EditWindow nowe = new EditWindow(ID);
            nowe.Left = this.Left ;
            nowe.Top = this.Top + 160;
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

                    ComboBoxServerList.ItemsSource = LoadJson<MyServersJSON>("MySerwers.txt").MyServers;
                    break;
                }
                i++;
            } 

        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo ID = ((Button)sender).CommandParameter as ServerInfo;

            if (MessageBox.Show("Czy na pewno chcesz usunąć ten serwer z listy?", "", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
            }
            else
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

                ComboBoxServerList.ItemsSource = LoadJson<MyServersJSON>("MySerwers.txt").MyServers;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBoxServerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var Combo = sender as ComboBox;
            Combo.Text = "";
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            ServerWindow okno = new ServerWindow();
            bool connected = true;
            if (connected)
            {
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
}
