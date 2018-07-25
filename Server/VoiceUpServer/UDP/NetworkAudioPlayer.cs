using System;
using NAudio.Wave;
using VoiceUpServer.UDP.Interface;

namespace VoiceUpServer.UDP
{
    class NetworkAudioPlayer : IDisposable
    {
        private readonly IAudioReceiver receiver;
        private readonly IAudioSender sender;

        public NetworkAudioPlayer(IAudioReceiver receiver,IAudioSender sender)
        {
            this.receiver = receiver;
            this.sender = sender;
            receiver.OnReceived(OnDataReceived);
        }

        void OnDataReceived(byte[] compressed)
        {
            //send dalej
        }

        public void Dispose()
        {
            receiver?.Dispose();
        }
    }
}