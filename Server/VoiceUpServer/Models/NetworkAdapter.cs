using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUpServer.Models
{
    class NetworkAdapter
    {
        private string networkAdapterName;

        public string Name
        {
            get { return networkAdapterName; }
            set { networkAdapterName = value; }
        }

        private string ip;

        public string IPAdress
        {
            get { return ip; }
            set { ip = value; }
        }

        public NetworkAdapter(string name, string ip)
        {
            this.networkAdapterName = name;
            this.ip = ip;
        }

        public NetworkAdapter()
        {

        }
    }
}
