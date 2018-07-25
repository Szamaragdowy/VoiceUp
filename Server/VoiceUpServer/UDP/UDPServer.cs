using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace VoiceUpServer.UDP
{
    class UDPServer
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private Socket _sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private static int multicastPort = 8080;
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        UdpClient udpclient = new UdpClient();
        UdpClient multicast = new UdpClient();
        IPEndPoint multicastIPEP = new IPEndPoint(IPAddress.Parse("192.168.1.106"), 5001);

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public UDPServer(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            _sendSocket.Bind(multicastIPEP);
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void SendMulticast(byte[] kek)
        {
            _sendSocket.Send(kek);
        }
        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                SendMulticast(so.buffer);
            }, state);
        }
    }
}
