using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using VoiceUpServer.Models;

namespace VoiceUpServer.AdditionalsWindows
{
    public partial class NetworkAdapters : Window
    {
        public string choicedIP="";

        private List<NetworkAdapter> InterfaceList;

        public NetworkAdapters()
        {
            InitializeComponent();
            InterfaceList = new List<NetworkAdapter>();
            LoadAdapters();
            NetworkAdaptersList.ItemsSource = InterfaceList;
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

        private void NetworkAdaptersList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                choicedIP = ((NetworkAdapter)(item.Content)).IPAdress;
                this.Close();
            }
        }
    }
}
