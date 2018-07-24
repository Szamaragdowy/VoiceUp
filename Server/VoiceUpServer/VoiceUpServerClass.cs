using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using VoiceUpServer.Models;
using VoiceUpServer.TCP;
using System.Text;
using System.Windows;
using System.Collections.Generic;
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
        private string _privateKey;
        static string CONTAINER_NAME = "MyContainerName";

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
        private void generateKeys()
        {
            CspParameters cspParameters = new CspParameters(1);
            cspParameters.KeyContainerName = CONTAINER_NAME;
            cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParameters.ProviderName = "Microsoft Strong Cryptographic Provider";
            var rsa = new RSACryptoServiceProvider(cspParameters);
            rsa.PersistKeyInCsp = true;

            this._publicKey = rsa.ToXmlString(false);
            this._privateKey = rsa.ToXmlString(true);
        }

        private  byte[] Encrypt(byte[] plain)
        {
            byte[] encrypted;
            int rsa_provider = 1;
            CspParameters cspParameters = new CspParameters(rsa_provider);
            cspParameters.KeyContainerName = CONTAINER_NAME;

            using (var rsa = new RSACryptoServiceProvider(1024, cspParameters))
            {
                encrypted = rsa.Encrypt(plain, true);
            }
            return encrypted;
        }

        private  byte[] Decrypt(byte[] encrypted)
        {
            byte[] decrypted;

            CspParameters cspParameters = new CspParameters();
            cspParameters.KeyContainerName = CONTAINER_NAME;

            using (var rsa = new RSACryptoServiceProvider(1024, cspParameters))
            {
                decrypted = rsa.Decrypt(encrypted, true);
            }
            return decrypted;
        }

        public  void DeleteKeyInCSP()
        {
            var cspParams = new CspParameters();
            cspParams.KeyContainerName = CONTAINER_NAME;
            var rsa = new RSACryptoServiceProvider(cspParams);
            rsa.PersistKeyInCsp = false;
            rsa.Clear();
        }

        public void start()
        {
            StartListening(_ServerIPAddress, _ServerPORT);
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
            lock (_itemsLock)
            {
                _usersList.Remove(user);
            }
        }

        public void AddUser (User user)
        {
            lock (_itemsLock)
            {
                _usersList.Add(user);
            }
        }

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

        private void AcceptCallback(IAsyncResult ar)
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
        }

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

                    string[] data = content.Split('/');
                    switch (data[0])
                    {
                        case "CYA":
                            //użytkownik się rozłącza
                            //usunąć z listy
                            //przerwać wątki
                            break;
                        case "JOIN":
                            //pierwsze połączenie użytkownika z serwerem
                            //SEND_P/klucz_publiczny
                            Send(handler, "SEND_P/"+ _publicKey+"/< EOF>");
                            break;
                        case "LOGIN":
                            //logowanie uzytkownika
                            //zaszyfrowane haslo serwera kluczem publicznym
                            //suma kontrolna
                            bool gooodPAss = true;
                            bool isNotFull = true;
                            if (gooodPAss)
                            {
                                if (isNotFull)
                                {
                                    AddUser(new User("name", "9.5.6.7"));
                                    Send(handler, "LOGIN_ACK/<EOF>");
                                    Send(handler, "AKT_USR/<EOF>");//lista użytkowników
                                }
                                else
                                {
                                    Send(handler, "FULL/<EOF>");
                                }
                            }
                            else
                            {
                                Send(handler, "LOGIN_NAK/<EOF>");
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
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                         new AsyncCallback(ReadCallback), state);
                }
            }
        }

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
    }
}
