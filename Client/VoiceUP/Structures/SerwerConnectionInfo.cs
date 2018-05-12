using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUP.Structures
{
    class SerwerConnectionInfo
    {
        public IPEndPoint IPAdres { get;}

        public string Name { get; set; }

        SerwerConnectionInfo(IPEndPoint ipAdres,String name)
        {
            this.IPAdres = ipAdres;
            this.Name = name;
        }
    }
}
