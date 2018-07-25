﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VoiceUP.UDP
{
    class ConnectWithServerUDP
    {
        //UdpClient client;
       // IPEndPoint serversIp;
        private volatile bool connected;
        private NetworkAudioPlayer player;
        private NetworkAudioSender audioSender;

        public ConnectWithServerUDP(string ip,int port,int indexDevice)
        {
            //this.client = new UdpClient();
            //this.serversIp = new IPEndPoint(IPAddress.Parse(ip), port);
            Connect(new IPEndPoint(IPAddress.Parse(ip), port), indexDevice);
        }

       /* public void SendBytes(byte[] data)
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
        }*/

        public void Close()
        {
            //client.Close();
        }

        private void Connect(IPEndPoint endPoint, int inputDeviceNumber)
        {
            OpusCodec codec = new OpusCodec();
            var receiver = new UdpAudioReceiver(endPoint.Port);
            var sender = new UdpAudioSender(endPoint);

            player = new NetworkAudioPlayer(codec, receiver);
            audioSender = new NetworkAudioSender(codec, inputDeviceNumber, sender);
            connected = true;
        }
    }
}
