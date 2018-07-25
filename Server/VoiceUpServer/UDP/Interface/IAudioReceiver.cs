using System;

namespace VoiceUpServer.UDP.Interface
{
    interface IAudioReceiver : IDisposable
    {
        void OnReceived(Action<byte[]> handler);
    }
}