using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUpServer.UDP
{
    class Class1
    {
        private Socket serverSocket = null;
        private List<EndPoint> clientList = new List<EndPoint>();
        private List<EndPoint> dataList = new List<EndPoint>();
        private byte[] byteData = new byte[1024];
        private int port = 4242;

        public List<EndPoint> DataList
        {
            private set { this.dataList = value; }
            get { return (this.dataList); }
        }

        public Class1(int port)
        {
            this.port = port;
            Start();
        }

        public void Start()
        {
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, this.port));
            EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
            this.serverSocket.BeginReceiveFrom(this.byteData, 0, this.byteData.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, newClientEP);
        }

        private void DoReceiveFrom(IAsyncResult iar)
        {
            try
            {
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                int dataLen = 0;
                byte[] data = null;
                try
                {
                    dataLen = this.serverSocket.EndReceiveFrom(iar, ref clientEP);
                    data = new byte[dataLen];
                    Array.Copy(this.byteData, data, dataLen);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                    this.serverSocket.BeginReceiveFrom(this.byteData, 0, this.byteData.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, newClientEP);
                }

                if (!this.clientList.Any(client => client.Equals(clientEP)))
                    this.clientList.Add(clientEP);
                SendToAll(data);

            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void SendTo(byte[] data, EndPoint clientEP)
        {
            try
            {
                this.serverSocket.SendTo(data, clientEP);
            }
            catch (System.Net.Sockets.SocketException)
            {
                this.clientList.Remove(clientEP);
            }
        }

        public void SendToAll(byte[] data)
        {
            foreach (var client in this.clientList)
            {
                this.SendTo(data, client);
            }
        }

        public void Stop()
        {
            this.serverSocket.Close();
            this.serverSocket = null;

            this.dataList.Clear();
            this.clientList.Clear();
        }
    }
}
