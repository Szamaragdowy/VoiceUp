using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUP.TCP
{
    class ConnectWithServerTCP
    {
        private TcpClient clientSocket;
        private NetworkStream serverStream;
        string _ip;
        int _port;

        public ConnectWithServerTCP(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
            clientSocket = new TcpClient();
            Console.WriteLine("Client Started");
        }

        public bool Connect()
        {
            try
            {
                clientSocket.Connect(_ip, _port);
                Console.WriteLine("Connected to the server");
                serverStream = clientSocket.GetStream();
                SendMessageToServer("LOGIN/<EOF>");
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't connect to the server");
                return false;
            }

            return true;
        }

        public void readTCP()
        {
            byte[] inStream = new byte[10025];
            serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
            string dataFromServer = Encoding.ASCII.GetString(inStream);
            Console.WriteLine("Data from Server : " + dataFromServer);
        }

        public void SendMessageToServer(string message)
        {
            byte[] outStream = Encoding.ASCII.GetBytes(message);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }
    }
}
