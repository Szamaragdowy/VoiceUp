using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VoiceUpServer.UDP
{
    class UDPServer
    {
        private Socket serverSocket = null;
        private List<EndPoint> clientList;
        private byte[] byteData = new byte[1024];
        private object _itemsLock;
        private int _port = 4242;


        public UDPServer(int port, List<EndPoint> list,object x)
        {
            this._port = port;
            this.clientList = list;
            this._itemsLock = x;

            Start();
        }

        public void Start()
        {
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, this._port));
            EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
            this.serverSocket.BeginReceiveFrom(this.byteData, 0, this.byteData.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, newClientEP);
            BindingOperations.EnableCollectionSynchronization(this.clientList, _itemsLock);

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
                IPEndPoint w;
                lock (_itemsLock) {
                    IPAddress x = ((System.Net.IPEndPoint)clientEP).Address;
                    w = new IPEndPoint(x, _port);
                    if (!this.clientList.Any(client => client.Equals(w)))
                    {
                        this.clientList.Add(w);
                    }
                }
                if (clientList.Count > 1)
                {
                    sendToRest(w, data);
                }
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
            lock (_itemsLock)
            {
                foreach (var client in this.clientList)
                {
                    this.SendTo(data, client);
                }
            }
        }

        public void sendToRest(IPEndPoint x, byte[] data)
        {
            lock (_itemsLock)
            {
                foreach (var client in this.clientList)
                {
                    if (!client.Equals(x))
                    {
                        this.SendTo(data, client);
                    }
                }
            }
        }

        public void Stop()
        {
            this.serverSocket.Close();
            this.serverSocket = null;

            this.clientList.Clear();
        }
    }
}
