using System;

namespace VoiceUP.UDP
{
    interface IAudioSender : IDisposable
    {
        void Send(byte[] payload);
    }
}