using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using VoiceUpServer.Models;
using System.Text;
using System.Windows.Data;
using System.Security.Cryptography;

namespace VoiceUpServer
{
    class VoiceUpServerClass
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private ObservableCollection<User> _usersList;
        private string _ServerName;
        private string _ServerIP;
        private IPAddress _ServerIPAddress;
        private int _ServerPORT;
        private int _MaxUsers;
        private object _itemsLock;
        private string _publicKey;
        private RSACryptoServiceProvider _rsa;
        private byte[] _buffer = new byte[1024];
        private Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ASCIIEncoding ByteConverter = new ASCIIEncoding();

        #region propertasy
        public ObservableCollection<User> ActualListOfUsers => _usersList;
        public string ServerName
        {
            get { return _ServerName; }
            set { _ServerName = value; }
        }
        public string ServerIP
        {
            get { return _ServerIP; }
            set { _ServerIP = value; }
        }
        public int ServerPort
        {
            get { return _ServerPORT; }
            set { _ServerPORT = value; }
        }
        public int MaxUsers
        {
            get { return _MaxUsers; }
            set { _MaxUsers = value; }
        }
        #endregion

        public VoiceUpServerClass(string Name, string ip, int port,int maxusers)
        {
            this._ServerName = Name;
            this._ServerIP = ip;
            this._ServerIPAddress = IPAddress.Parse(_ServerIP);
            this._ServerPORT = port;
            this._MaxUsers = maxusers;
            this._usersList = new ObservableCollection<User>();
            _itemsLock = new object();
            BindingOperations.EnableCollectionSynchronization(this._usersList, _itemsLock);
            generateKeys();
        }

        #region RSA
        private void generateKeys()
        {
            _rsa = new RSACryptoServiceProvider();
            this._publicKey = _rsa.ToXmlString(false);
        }


        #endregion

        public void start()
        {
            SetupServer();
            //StartListening(_ServerIPAddress, _ServerPORT);
        }

        public void stop()
        {
            try
            {
                //TcpServerThread.Abort();
            }
            catch(Exception e)
            {
               Console.WriteLine(e.Message);
               Console.WriteLine(" TCP >> " + "Koniec SERWERA ");
            }   
            Console.WriteLine(" TCP >> " + "exit");
            clearList();
        }

        private void clearList()
        {
            _usersList.Clear();
        }

        public void KickUser(User user)
        {
            if (user.workSocket.Connected)
            {
                Sendata(user.workSocket, "KICKED<VUP>");
                user.workSocket.Close();
            }
            _usersList.Remove(user);
        }

        public void AddUser (User user)
        {
            _usersList.Add(user);
        }
        
        public void ChangeUserMicrophoneStatus(User user)
        {
            if (!user.Mute)
            {
                user.Mute = true;
            }
            else
            {
                user.Mute = false;
            }
        }

        public void ChangeUserSoundStatus(User user)
        {
            if (!user.SoundOff)
            {
                user.SoundOff = true;
            }
            else
            {
                user.SoundOff = false;
            }
        }

        private void SetupServer()
        {
            Console.WriteLine("Setting up server . . .");
            _serverSocket.Bind(new IPEndPoint(_ServerIPAddress, _ServerPORT));
            _serverSocket.Listen(100);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectionCallBack), null);
            Console.WriteLine("Server ready.");
        }

        private void AcceptConnectionCallBack(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectionCallBack), null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket.Connected)
            {
                int received;
                try
                {
                    received = socket.EndReceive(ar);
                }
                catch (Exception)
                {

                    for (int i = 0; i < _usersList.Count; i++)
                    {
                        if (_usersList[i].workSocket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                        {
                            _usersList.RemoveAt(i);
                        }
                    }
                    return;
                }
                if (received != 0)
                {
                    byte[] dataBuf = new byte[received];
                    Array.Copy(_buffer, dataBuf, received);
                    string text = ByteConverter.GetString(dataBuf);

                    if (text.IndexOf("<EOF>") > -1)
                    {
                        Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", text.Length, text);

                        string[] data = text.Split(new[] { "<VUP>" }, StringSplitOptions.None);
                        switch (data[0])
                        {
                            case "CYA":
                                //użytkownik się rozłącza
                                //usunąć z listy
                                //przerwać wątki
                                break;
                            case "JOIN":
                                Sendata(socket, "SEND_P<VUP>" + _publicKey + "<VUP><EOF>");
                                break;
                            case "LOGIN":

                                byte[] bytesCypherText = Convert.FromBase64String(data[1]);
                                byte[] decryptedLogin = _rsa.Decrypt(bytesCypherText, false);
                                string decryptedLoginString = ByteConverter.GetString(decryptedLogin);

                                bytesCypherText = Convert.FromBase64String(data[2]);
                                byte[] decryptedPass = _rsa.Decrypt(bytesCypherText, false);
                                string decryptedPassnString = ByteConverter.GetString(decryptedPass);

                                bytesCypherText = Convert.FromBase64String(data[2]);
                                byte[] checksum = _rsa.Decrypt(bytesCypherText, false);
                                string checksumString = ByteConverter.GetString(checksum);                              

                                bool gooodPAss = true;
                                bool isNotFull = true;
                                if (gooodPAss)
                                {
                                    if (isNotFull)
                                    {
                                        User user = new User(socket);
                                        user.Name = decryptedLoginString;
                                        _usersList.Add(user);
                                        Sendata(socket, "LOGIN_ACK<VUP><EOF>");
                                    }
                                    else
                                    {
                                        Sendata(socket, "FULL<VUP><EOF>");
                                    }
                                }
                                else
                                {
                                    Sendata(socket, "LOGIN_NAK<VUP><EOF>");
                                }
                                break;
                            case "LIST":
                                /* for (int i = 0; i < _usersList.Count; i++)
                                {
                                    if (socket.RemoteEndPoint.ToString().Equals(_usersList[i].workSocket.RemoteEndPoint.ToString()))
                                    {
                                        //rich_Text.AppendText("\n" + __ClientSockets[i]._Name + ": " + text);
                                    }
                                }*/
                                StringBuilder builder = new StringBuilder();
                                builder.Append("AKT_USR<VUP>");
                                for (int i = 0; i < _usersList.Count; i++)
                                {
                                    builder.Append(_usersList[i].Name);
                                    builder.Append("<VUP>");
                                }
                                builder.Append("<EOF>");

                                Sendata(socket,builder.ToString());
                                //klient potwierdza swoją obecność
                                //reset timere
                                break;

                            case "CHECK_Y":
                                Sendata(socket, "AKT_USR<VUP><EOF>");
                                //klient potwierdza swoją obecność
                                //reset timere
                                break;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _usersList.Count; i++)
                        {
                            if (_usersList[i].workSocket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                            {
                                _usersList.RemoveAt(i);
                                Console.WriteLine("Số client đang kết nối: " + _usersList.Count.ToString());
                            }
                        }
                    }
                }
                try
                {
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }catch(SocketException e)
                {
                   
                }
            }
        }

        void Sendata(Socket socket, string noidung)
        {
            byte[] data = ByteConverter.GetBytes(noidung);
            Console.WriteLine("Send: " + ByteConverter.GetString(data));
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectionCallBack), null);
        }

        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }


        /*
       private void StartListening(IPAddress ip, int port)
       {
           byte[] bytes = new Byte[1024];
           IPEndPoint localEndPoint = new IPEndPoint(ip, port);
           Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

           try
           {
               listener.Bind(localEndPoint);
               listener.Listen(100);

               while (true)
               {
                   // Set the event to nonsignaled state.
                   allDone.Reset();

                   // Start an asynchronous socket to listen for connections.
                   Console.WriteLine("Waiting for a connection...");
                   listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                   // Wait until a connection is made before continuing.
                   allDone.WaitOne();
               }
           }
           catch (Exception e)
           {
               Console.WriteLine(e.ToString());
           }
       }
       */
        /*  private void AcceptCallback(IAsyncResult ar)
          {
              // Signal the main thread to continue.
              allDone.Set();

              // Get the socket that handles the client request.
              Socket listener = (Socket)ar.AsyncState;
              Socket handler = listener.EndAccept(ar);


              // Create the state object.
              StateObject state = new StateObject();
                  state.workSocket = handler;

              handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

          }*/
        /*
        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                    string[] data = content.Split('~');
                    switch (data[0])
                    {
                        case "CYA":
                            //użytkownik się rozłącza
                            //usunąć z listy
                            //przerwać wątki
                            break;
                        case "JOIN":
                            Send(handler, "SEND_P~" + _publicKey+ "~< EOF>");
                            break;
                        case "LOGIN":
                            string decryptedLogin = Decrypt(Encoding.ASCII.GetBytes(data[1]));
                            string decryptedPass = Decrypt(Encoding.ASCII.GetBytes(data[2]));
                            string checksum = Decrypt(Encoding.ASCII.GetBytes(data[3]));

                            bool gooodPAss = true;
                            bool isNotFull = true;
                            if (gooodPAss)
                            {
                                if (isNotFull)
                                {
                                    AddUser(new User(decryptedLogin, "9.5.6.7"));
                                    Send(handler, "LOGIN_ACK~<EOF>");
                                    Send(handler, "AKT_USR~<EOF>");//lista użytkowników
                                }
                                else
                                {
                                    Send(handler, "FULL~<EOF>");
                                }
                            }
                            else
                            {
                                Send(handler, "LOGIN_NAK~<EOF>");
                            }
                            break;
                        case "CHECK_Y":
                            //klient potwierdza swoją obecność
                            //reset timere
                            break;



                            //timer z odpytywaniem 2 minuty
                    }
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,new AsyncCallback(ReadCallback), state);
                }
            }
        }
        */
        /*
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        */
    }
}
