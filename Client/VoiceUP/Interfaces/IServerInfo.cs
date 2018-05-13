using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoiceUP.Interfaces
{
    interface IServerInfo
    {
        IPEndPoint IPAdres { get; }

        string Name { get; set; }

        void Edit(object sender, RoutedEventArgs e);

        void Delete(object sender, RoutedEventArgs e);

    }
}
