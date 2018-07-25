using System;

namespace VoiceUpServer.UDP.Interface
{
    interface IAudioSender : IDisposable
    {
        void Send(byte[] payload);
    }
}