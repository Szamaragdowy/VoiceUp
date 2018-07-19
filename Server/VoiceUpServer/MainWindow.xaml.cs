using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
            this.server = new Server("testowy","192.168.1.0",500,10);
            soundSender = new SoundSender();
           
            InitializeComponent();
            ListActualUsersOnServer.ItemsSource = server.ReturnActualListOfUsers;


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

        //wyjście z aplikacji
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
