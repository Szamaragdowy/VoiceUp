using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VoiceUpServer.Models;
using VoiceUpServer.UDP;

namespace VoiceUpServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server server;
        SoundSender soundSender;

        public MainWindow()
        {
            soundSender = new SoundSender();
           
            InitializeComponent();
            
            TextboxServerName.Text = "testowy";
            TextboxMaxUsers.Text = "4";



            TextblockIP.Text = GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            // LabelAmountOfActualUsers.Content = 


            /* HandleDataClass hdc = new HandleDataClass();

             Thread serverThread = new Thread(() => server.Listen());
             serverThread.Start();

             Thread datahandlerthread = new Thread(() => hdc.SubscribeToEvent(server));
             datahandlerthread.Start();*/

            /*serwer = new Server();
            soundSender = new SoundSender();
            soundSender.Receive(2000);*/
            /*
            while (true)
            {
                Thread.Sleep(100);
            }*/
        }

        //wyłączanie komuś dzwięku (przycisk) 
        private void ButtonHeadphones_Click(object sender, RoutedEventArgs e) 
        {
            var ButtonMicrophone = sender as Button;

            var user = ButtonMicrophone.DataContext as User;

            server.ChangeUserSoundStatus(user);
        }

        //wyciszenie mikrofonu (przycisk)
        private void ButtonMicrophone_Click(object sender, RoutedEventArgs e) 
        {
            var ButtonMicrophone = sender as Button;

            var user = ButtonMicrophone.DataContext as User;

            server.ChangeUserMicrophoneStatus(user);
        }

        //wyrzucenie kogoś (przycisk)
        private void ButtonKick_Click(object sender, RoutedEventArgs e) 
        {
            var ButtonMicrophone = sender as Button;
            var user = ButtonMicrophone.DataContext as User;

            server.KickUser(user);
        }

        //start serwera
        private void StartServer_Click(object sender, RoutedEventArgs e)
        { 
            if (StartButton.Content.ToString() == "Start")
            {
                bool isValid = true;
                HideAllWarnings();

                if (String.IsNullOrEmpty(TextboxMaxUsers.Text))
                {
                    WarningMaxUsers.Visibility = Visibility.Visible;
                    WarningMaxUsers.Content = "Pole nie moze być puste.";
                    isValid = false;
                }
                if (String.IsNullOrEmpty(TextboxServerName.Text))
                {
                    WarningServerName.Visibility = Visibility.Visible;
                    TextboxServerName.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("250"), Convert.ToByte("000"), Convert.ToByte("000")));
                    isValid = false;
                }
                if (String.IsNullOrEmpty(TextblockIP.Text))
                {
                    WarningIP.Visibility = Visibility.Visible;
                    WarningIP.Content = "Pole nie moze być puste.";
                    isValid = false;
                }
                else if(!isGoodIpAdress(TextblockIP.Text))
                {
                    WarningIP.Visibility = Visibility.Visible;
                    WarningIP.Content = "Błędny adres.";
                    isValid = false;
                }
                

                if (isValid)
                {
                    blockOptions();
                    string serverName = TextboxServerName.Text;
                    string ip = TextblockIP.Text;

                    this.server = new Server(serverName, ip, 500, 10);
                    ListActualUsersOnServer.ItemsSource = server.ActualListOfUsers;
                    server.start();
                    StartButton.Content = "Stop";
                }          
            }
            else
            {
                server.stop();
                UnBlockOptions();
                StartButton.Content = "Start";
            }
        }

        private bool isGoodIpAdress(string stringIP)
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if(ip.Address.ToString() == stringIP)
                        {
                            return true;
                        }

                    }
                }
            }
            return false; ;
        }

        private void UnBlockOptions()
        {
            OptionsGrid.IsEnabled = true;
            TextboxServerName.IsEnabled = true;
        }

        private void blockOptions()
        {
            TextboxServerName.IsEnabled = false;
            OptionsGrid.IsEnabled = false;
        }

        private void HideAllWarnings()
        {
            WarningMaxUsers.Visibility = Visibility.Hidden;
            WarningServerName.Visibility = Visibility.Hidden;
            WarningIP.Visibility = Visibility.Hidden;
            TextboxServerName.BorderBrush = new SolidColorBrush(Color.FromRgb(Convert.ToByte("89"), Convert.ToByte("000"), Convert.ToByte("000")));
        }

        //poprawność wpisywania maksymalna liczba uzytkowników
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextboxMaxUsers_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex("^([2-9]|[1-2][0-9]|30)$");
            e.Handled = regex.IsMatch(TextboxMaxUsers.Text);

            if (regex.IsMatch(TextboxMaxUsers.Text))
            {
                WarningMaxUsers.Visibility = Visibility.Hidden;
            }
            else
            {
                WarningMaxUsers.Visibility = Visibility.Visible;
                WarningMaxUsers.Content = "Proszę wpisać liczbę od 2 do 30";
                TextboxMaxUsers.Text = "";
            }
        }

        private void TextboxServerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextboxServerName.Text))
            {
                WarningServerName.Visibility = Visibility.Hidden;
            }
        }

        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
    }
}
