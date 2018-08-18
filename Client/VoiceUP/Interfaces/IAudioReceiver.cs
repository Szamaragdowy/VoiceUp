using System;

namespace VoiceUP.Interfaces
{
    interface IAudioReceiver : IDisposable
    {
        void OnReceived(Action<byte[]> handler);
    }
}