using System.Net;
using VoiceUpServer.UDP.Interface;


namespace VoiceUpServer.UDP
{
    class ServerUDP
    {
        private volatile bool connected;
        private NetworkAudioPlayer player;

        public ServerUDP(string ip,int port)
        {
            start(new IPEndPoint(IPAddress.Parse(ip), port));
        }

        private void start(IPEndPoint endPoint)
        {
            int inputDeviceNumber = 0;
            var receiver = new UdpAudioReceiver(endPoint.Port);
            var sender = new UdpAudioSender(new IPEndPoint(IPAddress.Parse("192.168.1.106"), endPoint.Port));

            player = new NetworkAudioPlayer(receiver,sender);
            connected = true;
        }
    }
}
