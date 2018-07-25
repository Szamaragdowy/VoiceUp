using System;

namespace VoiceUP.UDP
{
    interface IAudioReceiver : IDisposable
    {
        void OnReceived(Action<byte[]> handler);
    }
}