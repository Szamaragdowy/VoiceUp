using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUpServer.UDP
{
    class HandleDataClass
    {
        public void  SubscribeToEvent(Server server)
        {
            server.DataREceivedEvent += server_DataRecivedEvent;
        }

        void server_DataRecivedEvent(object sender, ReceivedDataArgs args)
        {
            Console.WriteLine("Received message from [{0}:{1}]:\r\n{2}",
                args.IPAddress.ToString(), args.Port.ToString(), Encoding.ASCII.GetString(args.ReceivedBytes));
        }
    }
}
