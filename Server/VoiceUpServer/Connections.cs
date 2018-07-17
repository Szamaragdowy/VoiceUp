using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace VoiceUpServer
{
    class Connections
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private static int multicastPort = 8080;
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        UdpClient udpclient = new UdpClient();
        UdpClient multicast = new UdpClient();
        IPEndPoint multicastIPEP = new IPEndPoint(IPAddress.Parse("239.0.0.222"), multicastPort);

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, state);
        }
        //funkcje dodawania i usuwania multicasta nie wiem jak rozbić multicasta :/
        /*
        public void AddToMulticastGroup(string addr)
        {
            IPAddress ip = IPAddress.Parse(addr);
            multicast.JoinMulticastGroup(ip);
        }
        public void DropFromMulticastGroup(string addr)
        {
            IPAddress ip = IPAddress.Parse(addr);
            multicast.DropMulticastGroup(ip);
        }
        */
        public void SendMulticast(string kek)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(kek);
            multicast.Send(bytes, bytes.Length, multicastIPEP);
            multicast.Close();
        }
        public void SendMulticast(byte[] kek)
        {
            multicast.Send(kek, kek.Length, multicastIPEP);
            multicast.Close();
        }
        //Receive and Send to another clients
        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                //Send to all users in multicast
                SendMulticast(so.buffer);
                //checking data
                //Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, state);
        }
    }
}
