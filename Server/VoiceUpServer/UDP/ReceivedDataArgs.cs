using System.Net;

namespace VoiceUpServer.UDP
{
    class ReceivedDataArgs
    {
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
        public byte[] ReceivedBytes;

        public ReceivedDataArgs(IPAddress ip, int port, byte[] data)
        {
            this.IPAddress = ip;
            this.Port = port;
            this.ReceivedBytes = data;
        }
    }
}
