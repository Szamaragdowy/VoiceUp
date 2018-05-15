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
        string IP { get; set; }

        string Port { get; set; }

        string Name { get; set; }

    }
}
