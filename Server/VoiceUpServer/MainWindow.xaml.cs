using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VoiceUpServer.Models;

namespace VoiceUpServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server serwer;

        public MainWindow()
        {
            InitializeComponent();
            serwer = new Server();
        }

        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.ItemsSource = serwer.ReturnList();
        }

        private void ButtonHeadphones_Click(object sender, RoutedEventArgs e) //wyłączanie komuś dzwięku (przycisk) 
        {
            var ButtonMicrophone = sender as Button;

            var user = ButtonMicrophone.DataContext as User;

            serwer.ChangeUserSoundStatus(user);
        }

        private void ButtonMicrophone_Click(object sender, RoutedEventArgs e) //wyciszenie mikrofonu (przycisk)
        {
            var ButtonMicrophone = sender as Button;

            var user = ButtonMicrophone.DataContext as User;

            serwer.ChangeUserMicrophoneStatus(user);
        }

        private void ButtonKick_Click(object sender, RoutedEventArgs e) //wyrzucenie kogoś (przycisk)
        {
            var ButtonMicrophone = sender as Button;
            var user = ButtonMicrophone.DataContext as User;

            serwer.KickUser(user);
        }

        private void ButtonUserEdit_Click(object sender, RoutedEventArgs e) //edycja?? 
        {
            throw new NotImplementedException();
        }

    }
}
