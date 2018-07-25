using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using VoiceUpServer.TCP;
using System.Security.Cryptography;

namespace VoiceUpServer.TCP
{
    public  class AsynchronousClient
    {
        private  ManualResetEvent connectDone = new ManualResetEvent(false);
        private  ManualResetEvent sendDone = new ManualResetEvent(false);
        private  ManualResetEvent receiveDone = new ManualResetEvent(false);
        private  bool connected = false;
        private string response = String.Empty;

        public bool TryToConnect(IPAddress ip, int port,string nick,string xd)
        {
            byte[] _nick = Encoding.ASCII.GetBytes(nick);
            byte[] _xd = Encoding.ASCII.GetBytes(xd);
            byte[] _checksum = Encoding.ASCII.GetBytes("997");
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ip, port);

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();


                Send(client, "JOIN~<EOF>");
                sendDone.WaitOne();

                Receive(client);
                receiveDone.WaitOne();

                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", response.Length, response);
                string[] data = response.Split('~');
                switch (data[0])
                {
                    case "SEND_P":
                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                        rsa.FromXmlString(data[1]);
                        byte[] encryptedLogin = rsa.Encrypt(_nick, true);
                        byte[] encryptedXd = rsa.Encrypt(_xd, true);
                        byte[] checksum = rsa.Encrypt(_checksum, true);


                        Send(client, "LOGIN~" + encryptedLogin + "~" + encryptedXd +"~"+ checksum+"~< EOF>");
                        sendDone.WaitOne();
                        break;
                    case "FULL":

                        break;
                    case "LOGIN_ACK":


                        //wair for AKT_USE
                        connected = true;
                        break;
                    case "LOGIN_NAK":

                        
                        break;
                }


                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return connected;
        }

        private  void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private  void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private  void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #region sending

        public  void Send(Socket client, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private  void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

         #endregion
    }
}