using System.Net;
using VoiceUP.Network.Codec;

namespace VoiceUP.Network.UDP
{
    class UDPManager
    {
        private NetworkAudioPlayer audioplayer;
        private NetworkAudioSender audioSender;

        public UDPManager(IPEndPoint endPoint, int inputDeviceNumber)
        {
            G722ChatCodec codec = new G722ChatCodec();
            var receiver = new UdpAudioReceiver(endPoint.Port);
            var sender = new UdpAudioSender(endPoint);

            audioplayer = new NetworkAudioPlayer(codec, receiver);
            audioSender = new NetworkAudioSender(codec, inputDeviceNumber, sender);
        }
        public void SoundOff()
        {
            audioplayer.SoundOff();
        }
        public void unSoundOff()
        {
            audioplayer.unSoundOff();
        }
        public void Mute()
        {
            audioSender.Mute();
        }
        public void unMute()
        {
            audioSender.unMute();
        }
        public void setNewMicrophone(int index)
        {
            audioSender.changeMicrophone(index);
        }

        public void Dispose()
        {
            audioplayer.Dispose();
            audioSender?.Dispose();
        }
    }
}
