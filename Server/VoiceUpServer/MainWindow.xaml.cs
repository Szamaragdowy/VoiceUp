using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace VoiceUpServer
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

        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<UserInfo> collection = new ObservableCollection<UserInfo>();

            collection.Add(new UserInfo("Marek"));
            collection.Add(new UserInfo("Sławomir"));
            collection.Add(new UserInfo("Ola"));
            collection.Add(new UserInfo("Zbyszek"));
            collection.Add(new UserInfo("Pioter"));
            collection.Add(new UserInfo("Mandaryna"));
            collection.Add(new UserInfo("Wojtek"));
            collection.Add(new UserInfo("Krzysztof"));

            var listbox = sender as ListBox;
             listbox.ItemsSource = collection;
        }

        private void ButtonHeadphones_Click(object sender, RoutedEventArgs e)
        {
            var ButtonHeadphones = sender as Button;


            if (ButtonHeadphones.Content == FindResource("Headphone_On"))
                {
                    ButtonHeadphones.Content = FindResource("Headphone_Off");
                }
            else
                {
                    ButtonHeadphones.Content = FindResource("Headphone_On");
                }
        }

        private void ButtonMicrophone_Click(object sender, RoutedEventArgs e)
        {
            var ButtonMicrophone = sender as Button;


            if (ButtonMicrophone.Content == FindResource("Microphone_On"))
            {
                ButtonMicrophone.Content = FindResource("Microphone_Off");
            }
            else
            {
                ButtonMicrophone.Content = FindResource("Microphone_On");
            }
        }
    }
}
