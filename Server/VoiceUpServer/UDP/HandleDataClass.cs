using System;
using System.Text;

namespace VoiceUpServer.UDP
{
    class HandleDataClass
    {
        public void  SubscribeToEvent(UDPListener server)
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
