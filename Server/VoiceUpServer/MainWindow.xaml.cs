using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VoiceUpServer.Models;
using VoiceUpServer.Network;

namespace VoiceUpServer
{
    public partial class MainWindow : Window
    {
        VoiceUpServerClass server;
        Thread ServerTCPthread;
        List<NetworkAdapter> InterfaceList;

        public MainWindow()
        {   
            InitializeComponent(); 
            TextboxServerName.Text = "VoiceUp Server";
            TextboxMaxUsers.Text = "4";
            TextblockPort.Text = "5000";
            InterfaceList = new List<NetworkAdapter>();
            LoadAdapters();
            NetworkAdaptersList.ItemsSource = InterfaceList;
            NetworkAdaptersList.SelectedIndex = 0;
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
                Regex regex = new Regex("^([2-9]|[1-2][0-9]|30)$");
                if (!regex.IsMatch(TextboxMaxUsers.Text))
                {
                    WarningMaxUsers.Visibility = Visibility.Visible;
                    WarningMaxUsers.Content = "Proszę wpisać liczbę od 2 do 30";
                    isValid = false;
                }



                if (String.IsNullOrEmpty(TextblockPort.Text))
                {
                    WarningPORT.Visibility = Visibility.Visible;
                    WarningPORT.Content = "Pole nie moze być puste.";
                    isValid = false;
                }
                else if (!checkIsPortFree(Int32.Parse(TextblockPort.Text)))
                {
                    WarningPORT.Visibility = Visibility.Visible;
                    WarningPORT.Content = "Port jest już zajęty.";
                    isValid = false;
                }

                if (isValid)
                {
                    blockOptions();
                    string serverName = TextboxServerName.Text;
                    string ip = (NetworkAdaptersList.SelectedValue as NetworkAdapter).IPAdress;
                    int port = Int32.Parse(TextblockPort.Text);
                    int maxuser = Int32.Parse(TextboxMaxUsers.Text);
                    string pass = PasswordPasswordBox.Password;

                    int udpport = 0;
                    for (int i = 5001; i < 6000; i++)
                    {
                        if (checkIsPortFree(i))
                        {
                            udpport = i;
                            break;
                        }
                    }

                    this.server = new VoiceUpServerClass(serverName, ip, port, maxuser, pass,udpport);
                    ListActualUsersOnServer.ItemsSource = server.ActualListOfUsers;


                    ServerTCPthread = new Thread(server.start);
                    ServerTCPthread.Start();

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
            WarningPORT.Visibility = Visibility.Hidden;
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
            }
        }

        private void TextboxServerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextboxServerName.Text))
            {
                WarningServerName.Visibility = Visibility.Hidden;
            }
        }

        private void LoadAdapters()
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.PrefixLength <= 32)
                        {
                            InterfaceList.Add(new NetworkAdapter(item.Description, ip.Address.ToString()));
                        }
                    }
                }
            }
        }

        private void TextblockPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextblockPort.Text == "") return;
            int x = Int32.Parse(TextblockPort.Text);

            if (x>=0 && x <65535)
            {
                WarningPORT.Visibility = Visibility.Hidden;
            }
            else
            {
                WarningPORT.Visibility = Visibility.Visible;
                WarningPORT.Content = "Port może mieć wartości od 0  do 65535";
                TextblockPort.Text = "";
            }
        }

        private bool checkIsPortFree(int port)
        {
            bool isAvailable = true;

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }
            return isAvailable;
        }

        private void Label_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PasswordTextbox.Visibility = Visibility.Visible;
            PasswordPasswordBox.Visibility = Visibility.Hidden;
            PasswordTextbox.Text = PasswordPasswordBox.Password;
        }

        private void Label_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PasswordTextbox.Visibility = Visibility.Hidden;
            PasswordPasswordBox.Visibility = Visibility.Visible;
        }

    }
}
