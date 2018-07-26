using System;
using NAudio.Wave;

namespace VoiceUP.UDP
{
    class NetworkAudioSender : IDisposable
    {
        private readonly INetworkChatCodec codec;
        private readonly IAudioSender audioSender;
        private readonly WaveIn waveIn;

        public NetworkAudioSender(INetworkChatCodec codec, int inputDeviceNumber, IAudioSender audioSender)
        {
            this.codec = codec;
            this.audioSender = audioSender;
            waveIn = new WaveIn();
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = inputDeviceNumber;
            waveIn.WaveFormat = codec.RecordFormat;
            waveIn.DataAvailable += OnAudioCaptured;
            waveIn.StartRecording();
        }

        public void changeMicrophone(int index)
        {
            waveIn.StopRecording();
            waveIn.BufferMilliseconds = 50;
            waveIn.DeviceNumber = index;
            waveIn.WaveFormat = codec.RecordFormat;
            waveIn.DataAvailable += OnAudioCaptured;
            waveIn.StartRecording();
        }

        void OnAudioCaptured(object sender, WaveInEventArgs e)
        {
            byte[] encoded = codec.Encode(e.Buffer, 0, e.BytesRecorded);
            audioSender.Send(encoded);
        }

        public void Dispose()
        {
            waveIn.DataAvailable -= OnAudioCaptured;
            waveIn.StopRecording();
            audioSender?.Dispose();
        }
    }
}