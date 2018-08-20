using System.Net;
using VoiceUP.Network.Codec;

namespace VoiceUP.Network.UDP
{
    class UDPManager
    {
        private NetworkAudioPlayer player;
        private NetworkAudioSender audioSender;

        public UDPManager(IPEndPoint endPoint, int inputDeviceNumber)
        {
            G722ChatCodec codec = new G722ChatCodec();
            var receiver = new UdpAudioReceiver(endPoint.Port);
            var sender = new UdpAudioSender(endPoint);

            player = new NetworkAudioPlayer(codec, receiver);
            audioSender = new NetworkAudioSender(codec, inputDeviceNumber, sender);
        }

        public void setNewMicrophone(int index)
        {
            audioSender.changeMicrophone(index);
        }

        public void Dispose()
        {
            player.Dispose();
            audioSender?.Dispose();
        }
    }
}
