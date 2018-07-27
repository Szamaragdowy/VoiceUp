using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VoiceUpServer.Models;

namespace VoiceUpServer.UDP
{
    class UDPServer
    {
        private Socket _ServerReceiveSocket = null;
        private Socket _ServerSendSocket = null;
        private ObservableCollection<User> clientList;
        private byte[] byteData = new byte[1024];
        private object _itemsLock;
        private int _port = 4242;


        public UDPServer(int port, ObservableCollection<User> list,object x)
        {
            this._port = port;
            this.clientList = list;
            this._itemsLock = x;

            Start();
        }

        public void Start()
        {
            this._ServerReceiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this._ServerReceiveSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this._ServerReceiveSocket.Bind(new IPEndPoint(IPAddress.Any, this._port));
            this._ServerSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this._ServerSendSocket.Bind(new IPEndPoint(IPAddress.Any, 0));


            EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
            this._ServerReceiveSocket.BeginReceiveFrom(this.byteData, 0, this.byteData.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, newClientEP);
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
                    dataLen = this._ServerReceiveSocket.EndReceiveFrom(iar, ref clientEP);
                    data = new byte[dataLen];
                    Array.Copy(this.byteData, data, dataLen);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                    this._ServerReceiveSocket.BeginReceiveFrom(this.byteData, 0, this.byteData.Length, SocketFlags.None, ref newClientEP, DoReceiveFrom, newClientEP);
                }

                if (true)//clientList.Count > 1)
                {
                    sendToRest(((IPEndPoint)clientEP).Address, data);
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
                this._ServerSendSocket.SendTo(data, clientEP);
            }
            catch (System.Net.Sockets.SocketException)
            {
                
            }
        }


        public void sendToRest(IPAddress from,byte[] data)
        {
            lock (_itemsLock)
            {
                foreach (var client in this.clientList)
                {
                    if (from.ToString() != client._ipendpoint.Address.ToString()) { 
                        if (!client.SoundOff)
                        {
                            this.SendTo(data, client._ipendpoint);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
            this._ServerReceiveSocket.Close();
            this._ServerReceiveSocket = null;

            this.clientList.Clear();
        }
    }
}
