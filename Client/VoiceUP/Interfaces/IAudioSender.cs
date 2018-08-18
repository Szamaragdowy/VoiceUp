using System;

namespace VoiceUP.Interfaces
{
    interface IAudioSender : IDisposable
    {
        void Send(byte[] payload);
    }
}