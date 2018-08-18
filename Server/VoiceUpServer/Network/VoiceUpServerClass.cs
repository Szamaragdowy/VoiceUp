using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using VoiceUpServer.Models;
using System.Text;
using System.Windows.Data;
using System.Security.Cryptography;
using VoiceUpServer.Network.UDP;

namespace VoiceUpServer.Network
{
    class VoiceUpServerClass
    {
        public static ManualResetEvent kickMsgSended = new ManualResetEvent(false);
        public static ManualResetEvent cyaMsgSended = new ManualResetEvent(false);      
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
        private String _password;
        private bool ServerClosed;
        private int _UDPPort;
        byte[] _UDPBuffer = new byte[1024];

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

        public VoiceUpServerClass(string Name, string ip, int port,int maxusers,string password,int udpPort)
        {
            this._UDPPort = udpPort;
            this._ServerName = Name;
            this._ServerIP = ip;
            this._ServerIPAddress = IPAddress.Parse(_ServerIP);
            this._ServerPORT = port;
            this._MaxUsers = maxusers;
            this._usersList = new ObservableCollection<User>();
            this._password = password;
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
            startTCP();
            startUDP();
        }

        public void startUDP()
        {
            Console.WriteLine("UDP   ->    Setting up server . . .");
            UDPServer udp = new UDPServer(_UDPPort, _usersList, _itemsLock);
        }

        private void UDPReceiveCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket.Connected)
            {
                int received;
                try
                {
                    received = socket.EndReceive(ar);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR UDP CALLBACK" + e.Message);
                    return;
                }

                if (received != 0)
                {
                    byte[] dataBuf = new byte[received];
                    Array.Copy(_UDPBuffer, dataBuf, received);
                    string text = ByteConverter.GetString(dataBuf);

                }
                try
                {

                    socket.BeginReceive(_UDPBuffer, 0, _UDPBuffer.Length, SocketFlags.None, new AsyncCallback(UDPReceiveCallBack), socket);

                }
                catch (SocketException e)
                {
                    Console.WriteLine("SOCKET UDP" + e.Message);
                }
            }
        }

        public void startTCP()
        {
            Console.WriteLine("TCP   ->     Setting up server . . .");
            _serverSocket.Bind(new IPEndPoint(_ServerIPAddress, _ServerPORT));
            _serverSocket.Listen(100);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectionCallBack), null);
            Console.WriteLine("Server ready.");
        }

        public void stop()
        {
            try
            {
                foreach (var user in _usersList)
                {
                    if (user.workSocket.Connected)
                    {
                        Sendata(user.workSocket, "CYA<VUP><EOF>");
                        cyaMsgSended.WaitOne();
                        cyaMsgSended.Reset();
                        user.workSocket.Close();
                    }  
                }
                 _usersList.Clear();
                if(_serverSocket.Connected) _serverSocket.Disconnect(true);
                ServerClosed = true;
                _serverSocket.Dispose();
                _serverSocket.Close();
            }
            catch(Exception e)
            {
               Console.WriteLine(e.Message);
               Console.WriteLine(" TCP >> " + "Koniec SERWERA ");
            }   
            Console.WriteLine(" TCP >> " + "exit");
        }

        public void KickUser(User user)
        {
            if (user.workSocket.Connected)
            {
                Sendata(user.workSocket, "KICKED<VUP><EOF>");
                kickMsgSended.WaitOne();
                kickMsgSended.Reset();
                user.workSocket.Close();
            }
            _usersList.Remove(user);
            sendToAll(actuallist());
        }

        public void UserLeft(User user)
        {
            if (user.workSocket.Connected)
            {
                user.workSocket.Close();
            }
            _usersList.Remove(user);
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

        private void AcceptConnectionCallBack(IAsyncResult ar)
        {
            try
            {
                if(!ServerClosed)
                {
                    Socket socket = _serverSocket.EndAccept(ar);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                    _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectionCallBack), null);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }   
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
                                for (int i = 0; i < _usersList.Count; i++)
                                {
                                    if (_usersList[i].workSocket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                                    {
                                        _usersList[i].workSocket.Close();
                                        _usersList.RemoveAt(i);
                                    }
                                }
                                sendToAll(actuallist());
                                break;
                            case "JOIN":
                                Sendata(socket, "SEND_P<VUP>"+_ServerName+ "<VUP>"+ _publicKey + "<VUP><EOF>");
                                break;
                            case "LOGIN":

                                byte[] bytesCypherText = Convert.FromBase64String(data[1]);
                                byte[] decryptedLogin = _rsa.Decrypt(bytesCypherText, false);
                                string decryptedLoginString = ByteConverter.GetString(decryptedLogin);

                                bytesCypherText = Convert.FromBase64String(data[2]);
                                byte[] decryptedPass = _rsa.Decrypt(bytesCypherText, false);
                                string decryptedPassnString = ByteConverter.GetString(decryptedPass);

                                bytesCypherText = Convert.FromBase64String(data[3]);
                                byte[] checksum = _rsa.Decrypt(bytesCypherText, false);
                                string checksumString = ByteConverter.GetString(checksum);                              

                                if(checksumString != "997"){
                                    Sendata(socket, "BAD_CHECKSUM<VUP><EOF>");
                                }
                                else
                                {
                                    if (_password== decryptedPassnString)
                                    {
                                        if (_usersList.Count < _MaxUsers)
                                        {
                                            string login = decryptedLoginString;

                                            int x = 1;
                                            for (int i = 0; i < _usersList.Count; i++)
                                            { 
                                               if ( _usersList[i].Name == login)
                                                {
                                                    login = decryptedLoginString + x;
                                                    x++;
                                                }
                                            }

                                            User user = new User(socket,_UDPPort);
                                            user.Name = login;
                                            _usersList.Add(user);
                                            Sendata(socket, "LOGIN_ACK<VUP>"+ _UDPPort.ToString()+"<VUP><EOF>");
                                            sendToAll(actuallist());
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
                                }
                                break;
                            case "CHECK_Y":
                                Sendata(socket, actuallist());
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
                    if (socket.Connected) { 
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                     }
                }catch(SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private string actuallist()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("AKT_USR<VUP>");
            for (int i = 0; i < _usersList.Count; i++)
            {
                builder.Append(_usersList[i].Name);
                builder.Append("<VUP>");
            }
            builder.Append("<EOF>");

            return builder.ToString();
        }

        private void sendToAll(string msg)
        {
            for (int i = 0; i < _usersList.Count; i++)
            {
                Sendata(_usersList[i].workSocket, msg);
            }
        }

        void Sendata(Socket socket, string noidung)
        {
            if (!ServerClosed)
            {
                byte[] data = ByteConverter.GetBytes(noidung);
                Console.WriteLine("Send: " + noidung);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);

                _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectionCallBack), null);
            }
        }

        private void SendCallback(IAsyncResult AR)
        {
            if (!ServerClosed) { 
            Socket socket = (Socket)AR.AsyncState;
                if (socket.Connected)
                {
                    socket.EndSend(AR);
                    kickMsgSended.Set();
                    cyaMsgSended.Set();
                }
            }
        }
    }
}
