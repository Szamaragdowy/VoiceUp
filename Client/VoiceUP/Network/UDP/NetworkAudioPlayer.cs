using System;
using NAudio.Wave;
using VoiceUP.Interfaces;

namespace VoiceUP.Network.UDP
{
    class NetworkAudioPlayer : IDisposable
    {
        private readonly INetworkChatCodec codec;
        private readonly IAudioReceiver receiver;
        private readonly IWavePlayer waveOut;
        private readonly BufferedWaveProvider waveProvider;

        public NetworkAudioPlayer(INetworkChatCodec codec, IAudioReceiver receiver)
        {
            this.codec = codec;
            this.receiver = receiver;
            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(codec.RecordFormat);
            waveOut.Init(waveProvider);
            waveOut.Play();
            receiver.OnReceived(OnDataReceived);
        }

        void OnDataReceived(byte[] compressed)
        {
            byte[] decoded = codec.Decode(compressed, 0, compressed.Length);
            waveProvider.AddSamples(decoded, 0, decoded.Length);
        }

        public void Dispose()
        {
            receiver?.Dispose();
            waveOut?.Stop();
           
        }

    }
}