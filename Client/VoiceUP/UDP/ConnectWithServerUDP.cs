using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VoiceUP.UDP
{
    class ConnectWithServerUDP
    {
        UdpClient client;
        IPEndPoint serversIp;

        public ConnectWithServerUDP(string ip,int port)
        {
            this.client = new UdpClient();
            this.serversIp = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public bool Connect()
        {
            try
            {
                client.Connect(this.serversIp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public void SendBytes(byte[] data)
        {
            try
            {
                client.Connect(this.serversIp);
                client.Send(data, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Błąd wysyłania danych" + data);
            }
        }
    }
}
