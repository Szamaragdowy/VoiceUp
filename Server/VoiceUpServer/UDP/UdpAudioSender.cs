using System.Net;
using System.Net.Sockets;
using VoiceUpServer.UDP.Interface;

namespace VoiceUpServer.UDP
{
    class UdpAudioSender : IAudioSender
    {
        private readonly UdpClient udpSender;
        public UdpAudioSender(IPEndPoint endPoint)
        {
            udpSender = new UdpClient();
            udpSender.Connect(endPoint);
        }

        public void Send(byte[] payload)
        {
            udpSender.Send(payload, payload.Length);
        }

        public void Dispose()
        {
            udpSender?.Close();
        }
    }
}