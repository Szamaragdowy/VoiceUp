using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ServerInfo first = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.168.0.1"), 4578), "Super serwer");
            ServerInfo first2 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.152.0.1"), 4578), "WOW");
            ServerInfo first3 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.130.0.1"), 4578), "Super serwer2");
            ServerInfo first4 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.05.0.1"), 4578), "Super serwer3");
            ServerInfo first5 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.4.0.1"), 4578), "Super serwer4");
            ServerInfo first6 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.4.0.1"), 4578), "Super serwer4");
            ServerInfo first7 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.4.0.1"), 4578), "Super serwer4");
            ServerInfo first8 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.4.0.1"), 4578), "Super serwer4");
            ServerInfo first9 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.4.0.1"), 4578), "Super serwer4");
            ServerInfo first10 = new ServerInfo(new IPEndPoint(IPAddress.Parse("192.4.0.1"), 4578), "Super serwer4");

            List<ServerInfo> lista = new List<ServerInfo>();

            lista.Add(first);
            lista.Add(first2);
            lista.Add(first3);
            lista.Add(first4);
            lista.Add(first5);
            lista.Add(first6);
            lista.Add(first7);
            lista.Add(first8);
            lista.Add(first9);
            lista.Add(first10);

            var Combo = sender as ComboBox;
            Combo.ItemsSource = lista;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedComboItem = sender as ComboBox;
            ServerInfo name = selectedComboItem.SelectedItem as ServerInfo ;


            if (name != null)
            {
                selectedComboItem.Text = name.IPAdres.ToString();
            }

        }


        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            ServerInfo ID = ((Button)sender).CommandParameter as ServerInfo;
            EditWindow nowe = new EditWindow(ID);
            nowe.Left = this.Left ;
            nowe.Top = this.Top + 160;
            nowe.ShowDialog();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            object ID = ((Button)sender).CommandParameter;


            MessageBox.Show("wow");
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
