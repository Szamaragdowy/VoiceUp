using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VoiceUP.UDP
{
    class ConnectWithServerUDP
    {
        //UdpClient client;
       // IPEndPoint serversIp;
        private volatile bool connected;
        private NetworkAudioPlayer player;
        private NetworkAudioSender audioSender;

        public ConnectWithServerUDP(string ip,int port,int indexDevice)
        {
            Connect(new IPEndPoint(IPAddress.Parse(ip), port), indexDevice);
        }

        public void Close()
        {
            //client.Close();
        }

        private void Connect(IPEndPoint endPoint, int inputDeviceNumber)
        {
            G722ChatCodec codec = new G722ChatCodec();
            var receiver = new UdpAudioReceiver(endPoint.Port);
            var sender = new UdpAudioSender(endPoint);

            player = new NetworkAudioPlayer(codec, receiver);
            audioSender = new NetworkAudioSender(codec, inputDeviceNumber, sender);
            connected = true;
        }
    }
}
