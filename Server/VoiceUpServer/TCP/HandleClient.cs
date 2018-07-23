using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoiceUpServer.TCP
{
    class HandleClient
    {
        TcpClient _ClientSocket;
        string _ClientNumber;
        NetworkStream networkStream;
        Byte[] bytesFrom = new byte[10025];
        Byte[] sendBytes = new byte[10025];

        public void startClient(TcpClient inClientSocket, string clientNumber)
        {
            this._ClientSocket = inClientSocket;
            this.networkStream = _ClientSocket.GetStream(); ;
            this._ClientNumber = clientNumber;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            string dataFromClient = null;
            string serverResponse = null;

            while (true)
            {
                try
                {
                    networkStream.Read(bytesFrom, 0, (int)_ClientSocket.ReceiveBufferSize);
                    dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                    Console.WriteLine(" TCP Server >> " + "From client-" + _ClientNumber + dataFromClient);
                    serverResponse = protocol(dataFromClient);
                    Console.WriteLine(" TCP Server >> " + "To Client from Server-" + _ClientNumber + serverResponse);

                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }
            }
        }   

        private string protocol(string receivedData)
        {
            bool loged = false;
            bool temp = false;

            string[] datas = receivedData.Split('/');
            if (datas[0] == "CYA")
            {
               // loged = false;
                return "CYA";
            }
            if (loged == true)
            {
                return "AKT_USR/";//+ string z użytkownikami po /
            }
            switch (datas[0])
            {
                case "JOIN":
                    //wyslanie klucza publicznego
                    return "SEND_P/" + "";//zamiast tego po + jakiś klucz publiczny wygenerowany
                case "LOGIN":
                    if (temp)
                    {
                        loged = true;
                        //sprawdzenie czy serwer nie jest pełny jesli tak
                        //return "FULL";
                        //jeśli nie
                        return "LOGIN_ACK";
                    }
                    else return "LOGIN_NAK";
                case "CHECK_Y":
                    return "AKT_USR/";//+ string z użytkownikami po /
            }
            return "Something goes wrong";//okienko messege box
        }
    }
}
