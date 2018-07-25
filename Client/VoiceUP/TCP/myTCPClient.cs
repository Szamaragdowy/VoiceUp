using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using VoiceUP.Structures;

namespace VoiceUP.TCP
{
    public class myTCPClient
    {
        ASCIIEncoding ByteConverter = new ASCIIEncoding();
        NetworkStream _stream;
        TcpClient _tcpClient;
        bool connected = false;

        public myTCPClient(string _ServerIPAddress, int _ServerPORT)
        {
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
                    rsa.FromXmlString(data[1]);
                    
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
                    case "FULL":
                        resultOfConnecting = "FULL";
                        break;
                    case "LOGIN_ACK":
                        resultOfConnecting = "LOGIN_ACK";
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

        public ObservableCollection<UserInfo> GetCurrentUserList()
        {
            ObservableCollection<UserInfo> resultList= new ObservableCollection<UserInfo>();

            if (!connected)
            {
                Console.WriteLine("Nie udało się połączyć z serwerem.");
            }

            sendMsg("LIST<VUP><EOF>");

            string responseKEY = receiveMsg();

            string[] data = responseKEY.Split(new[] { "<VUP>" }, StringSplitOptions.None);

            if (data[0] == "AKT_USR")
            {
                for(int i = 1; i < data.Length - 1; i++)
                {
                    resultList.Add(new UserInfo(data[i]));
                }
            }
            else
            {
                throw new Exception("Somethink goes wrong! Not Receive  AKT_USR");
            }

            return resultList;
        }

        private void sendMsg(string msg)
        {
            Byte[] data = ByteConverter.GetBytes(msg);
            _stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent: {0}", msg);
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

        public void Close()
        {
            _stream.Close();
            _tcpClient.Close();
        }
    }
}
