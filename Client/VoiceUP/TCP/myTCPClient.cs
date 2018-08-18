using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Data;
using VoiceUP.Structures;
using VoiceUP.UDP;
using VoiceUP.Models;

namespace VoiceUP.TCP
{
    public class myTCPClient
    {
        ASCIIEncoding ByteConverter = new ASCIIEncoding();
        NetworkStream _stream;
        TcpClient _tcpClient;
        bool connected = false;
        bool disconected = false;
        string _serverName;
        private object _itemsLock;
        string ip;
        string port;
        private ObservableCollection<UserInfo> _collection;
        Func<bool> delegatekick;
        Func<bool> delegatesya;
        ConnectWithServerUDP _ConnectWithServerUDP;
        int _PortToUDP;
        private int oldMicIndex;


        public myTCPClient(string _ServerIPAddress, int _ServerPORT)
        {
            this.ip = _ServerIPAddress;
            this.port = _ServerPORT.ToString();
            this._collection = new ObservableCollection<UserInfo>();
            _itemsLock = new object();
            BindingOperations.EnableCollectionSynchronization(this._collection, _itemsLock);
            try
            {
                _tcpClient = new TcpClient(_ServerIPAddress, _ServerPORT);
                _stream = _tcpClient.GetStream();
                connected = true;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        public void maybeMicrophoneChanged(SoundManager sm)
        {
            if (oldMicIndex != sm.microphoneIndex)
            {
                this._ConnectWithServerUDP.setNewMicrophone(sm.microphoneIndex);
            }
        }

        public void startUDP(int index)
        {
            this.oldMicIndex = index;
            this._ConnectWithServerUDP = new ConnectWithServerUDP(ip, _PortToUDP, index);
        }



        public void setDeleagats(Func<bool> kick, Func<bool> sya)
        {
            this.delegatekick = kick;
            this.delegatesya = sya;
        }

        public ObservableCollection<UserInfo> getList()
        {
            return _collection;
        }

        public string GetIPAndPort()
        {
            return ip + ":" + port;
        }

        public string Connect(string nick, string xd)
        {
            string resultOfConnecting = String.Empty;
            byte[] _nick = ByteConverter.GetBytes(nick);
            byte[] _xd = ByteConverter.GetBytes(xd);
            byte[] _checksum = ByteConverter.GetBytes("997");

            try
            {
                if (!connected)
                {
                    resultOfConnecting = "Nie udało się połączyć z serwerem.";
                    return resultOfConnecting;
                }
                sendMsg("JOIN<VUP><EOF>");

                string responseKEY = receiveMsg();

                string[] data = responseKEY.Split(new[] { "<VUP>" }, StringSplitOptions.None);

                if(data[0]== "SEND_P")
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    _serverName = data[1];
                    rsa.FromXmlString(data[2]);
                    
                    byte[] encryptedLogin = rsa.Encrypt(_nick, false);
                    byte[] encryptedXd = rsa.Encrypt(_xd, false);
                    byte[] checksum = rsa.Encrypt(_checksum, false);

                    string encryptedLoginString = Convert.ToBase64String(encryptedLogin);
                    string encryptedXdString = Convert.ToBase64String(encryptedXd);
                    string checksumString = Convert.ToBase64String(checksum);

                    sendMsg("LOGIN<VUP>" + encryptedLoginString+ "<VUP>" + encryptedXdString+ "<VUP>" + checksumString+ "<VUP><EOF>");
                }
                else
                {
                    throw new Exception("Somethink goes wrong! Not SEND_P");
                }

                string responseLogin = receiveMsg();
                string[] loginResponse = responseLogin.Split(new[] { "<VUP>" }, StringSplitOptions.None);

                switch (loginResponse[0])
                {
                    case "BAD_CHECKSUM":
                        resultOfConnecting = "BAD_CHECKSUM";
                        break;
                    case "FULL":
                        resultOfConnecting = "FULL";
                        break;
                    case "LOGIN_ACK":
                        resultOfConnecting = "LOGIN_ACK/" + _serverName;
                        this._PortToUDP = Int32.Parse(loginResponse[1]);
                        StateObject state = new StateObject();
                        _stream.BeginRead(state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), state);
                        break;
                    case "LOGIN_NAK":
                        resultOfConnecting = "LOGIN_NAK";
                        break;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            return resultOfConnecting;
        }

        private void ReadCallback(IAsyncResult ar)
        {
            if (disconected)
            {
                return;
            }

            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            int bytesRead=0;
            try
            {
                bytesRead = _stream.EndRead(ar);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                delegatesya();
                return;
            }

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    string[] data = content.Split(new[] { "<VUP>" }, StringSplitOptions.None);
                    switch (data[0])
                    {
                        case "KICKED":
                            disconected = true;
                            delegatekick();
                            break;
                        case "CHECK":
                            sendMsg("CHECK_Y<VUP><EOF>");
                            break;
                        case "AKT_USR":
                            lock (_itemsLock) { 
                                _collection.Clear();
                                for (int i = 1; i < data.Length - 1; i++)
                                {
                                    _collection.Add(new UserInfo(data[i]));
                                }
                            }
                            break;
                        case "CYA":
                            disconected = true;
                            delegatesya();
                            break;
                    }
                }
                else
                {
                    _stream.BeginRead(state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), state);
                }
            }
            state.sb.Clear();
            if (!disconected)
            {
                _stream.BeginRead(state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), state);
            }
        }

        public void Discconect()
        {
            sendMsg("CYA<VUP><EOF>");
            disconected = true;
            closeAfterDisconect();
            _stream.Close();
            _tcpClient.Close();
            this._ConnectWithServerUDP.Dispose();
        }

        public void closeAfterDisconect()
        {
            disconected = true;
            _stream.Close();
            _tcpClient.Close();
            this._ConnectWithServerUDP.Dispose();
        }

        private void sendMsg(string msg)
        {
            if (!disconected)
            {
                Byte[] data = ByteConverter.GetBytes(msg);
                _stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", msg);
            }
        }

        private string receiveMsg()
        {
            Byte[] data = new Byte[1024];
            String responseData = String.Empty;

            Int32 bytes = _stream.Read(data, 0, data.Length);
            responseData = ByteConverter.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);
            return responseData;
        }
    }
}
