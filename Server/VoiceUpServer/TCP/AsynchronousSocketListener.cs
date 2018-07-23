using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace VoiceUpServer.TCP
{
    public static class AsynchronousSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static void StartListening(IPAddress ip, int port)
        {
            byte[] bytes = new Byte[1024];
            IPEndPoint localEndPoint = new IPEndPoint(ip, port);
            Socket listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

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
                    listener.BeginAccept(new AsyncCallback(AcceptCallback),listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);

                    string[] data = content.Split('/');
                    switch(data[0])
                    {
                        case "CYA":
                            //użytkownik się rozłącza
                            //usunąć z listy
                            //przerwać wątki
                            break;
                        case "JOIN":
                            //pierwsze połączenie użytkownika z serwerem

                            //SEND_P/klucz_publiczny
                            Send(handler, "SEND_P/Public_key<EOF>");
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

        private static void SendCallback(IAsyncResult ar)
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

        private static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }
    }
}