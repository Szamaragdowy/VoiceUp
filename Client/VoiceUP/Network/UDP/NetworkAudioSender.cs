﻿using System;
using NAudio.Wave;
using VoiceUP.Interfaces;

namespace VoiceUP.Network.UDP
{
    class NetworkAudioSender : IDisposable
    {
        private readonly INetworkChatCodec codec;
        private readonly IAudioSender audioSender;
        private WaveIn waveIn;
        private bool serverMute;
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
        public void Mute()
        {
            waveIn.StopRecording();
            waveIn.Dispose();
            waveIn = null;
        }
        public void unMute()
        {
            waveIn = new WaveIn();
            waveIn.BufferMilliseconds = 50;
            waveIn.WaveFormat = codec.RecordFormat;
            waveIn.DataAvailable += OnAudioCaptured;
            waveIn.StartRecording();
        }
        public void ServerMute()
        {
            serverMute = true;
        }
        public void ServerUnMute()
        {
            serverMute = false;
        }


        void OnAudioCaptured(object sender, WaveInEventArgs e)
        {
            if (serverMute)
            {

            }
            else
            {
                byte[] encoded = codec.Encode(e.Buffer, 0, e.BytesRecorded);
                audioSender.Send(encoded);
            }
        }

        public void Dispose()
        {
            if (waveIn != null)
            {
                waveIn.DataAvailable -= OnAudioCaptured;
                waveIn.StopRecording();
            }
            audioSender?.Dispose();
        }
    }
}