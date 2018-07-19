using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace VoiceUpServer.UDP
{
    class UDPListener
    {
        public void Listen()
        {
            UdpClient listener = new UdpClient(5000);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 5000);

            while (true)
            {

                byte[] data = listener.Receive(ref serverEP);
                RaiseDataReceived(new ReceivedDataArgs(serverEP.Address,
                    serverEP.Port, data));
                //raise event
            }
        }

        public delegate void DataReceived(object sender, ReceivedDataArgs args);

        public event DataReceived DataREceivedEvent;

        private void RaiseDataReceived(ReceivedDataArgs args)
        {
            if (DataREceivedEvent != null)
                DataREceivedEvent(this, args);
        }
    }
}
