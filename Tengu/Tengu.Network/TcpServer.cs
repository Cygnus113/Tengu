using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tengu.Utility;

namespace Tengu.Network
{
    // Base class for Asynchronous TCP servers
    public class TcpServer
    {
        // A nested class used for reading client data asynchronously
        protected class ClientState
        {
            // Client Socket.  
            public Socket WorkSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 1024;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Packet Recieved
            public Packet Packet = new Packet();
        }

        // Event Signals for async threads
        protected static ManualResetEvent AllDone = new ManualResetEvent(false);
        protected static ManualResetEvent ConnectDone = new ManualResetEvent(false);
        protected static ManualResetEvent SendDone = new ManualResetEvent(false);

        // User defined server tag
        protected string Tag;

        // Is server running
        protected bool Running;

        public TcpServer(string Tag = "Tengu")
        {
            // Name of Server
            this.Tag = Tag;
        }
        public void StartServer(int localPort, string localAddressString)
        {
            IPAddress localAddress = IPAddress.Parse(localAddressString);

            ConsoleHelper.WriteLine(Tag + " Server is starting on port " + localPort, true);
            Running = true;

            // Establish local endpoint
            IPEndPoint localEndPoint = new IPEndPoint(localAddress, localPort);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(localAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                // Accept Clients continuously
                while (Running)
                {
                    // Set the event to nonsignaled state.  
                    AllDone.Reset();
                    ConsoleHelper.WriteLine("Listening for connections...", ConsoleColor.Magenta);
                   
                    // Start listening and bind callback method  
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine("An Error occured in server", false);
                Console.WriteLine(e.ToString());
            }
        }
        protected void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            AllDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the ClientState object and begin recieving data 
            ClientState client = new ClientState();
            client.WorkSocket = handler;
            handler.BeginReceive(client.buffer, 0, ClientState.BufferSize,
                0, new AsyncCallback(ReadCallback), client);
            OnClientConnect(client);
        }
        protected virtual void OnClientConnect(ClientState client )
        {

        }
        protected void ReadCallback(IAsyncResult ar)
        {
            // Get ClientState and Socket from Async Result
            ClientState client = (ClientState)ar.AsyncState;
            Socket handler = client.WorkSocket;

            // Amount of read data from the client socket.  
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // Copy data from client buffer to packet body
                Buffer.BlockCopy(client.buffer, 0, client.Packet.Body,
                    0, bytesRead);

                // If we have the ID's and the length loaded into Packet Body, read them
                if (bytesRead >= 6)
                {
                    client.Packet.ReadHeader();
                    client.Packet.Source = client.WorkSocket;

                    // If all the data has been read, handle the message
                    if (bytesRead >= client.Packet.Length)
                    {
                        HandleMessage(client.Packet);
                    }
                }
            }
            // Continue recieving data again from this client
            if (handler.Connected)
            {
                handler.BeginReceive(client.buffer, 0, ClientState.BufferSize,
                    0, new AsyncCallback(ReadCallback), client);
            }
        }
        protected virtual void HandleMessage(Packet packet)
        {

        }
        public void Connect(string ip, int serverPort, ref Socket outSocket)
        {
            // Get IP Address
            IPAddress ipAddress = IPAddress.Parse(ip.Trim());
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, serverPort);

            // Save the socket for user reference later  
            outSocket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            outSocket.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), outSocket);
            ConnectDone.WaitOne();
        }
        protected void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the StateObject.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Connected to Remote Endpoint",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                ConnectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void Send(Socket client, Packet packet)
        {
            // Get bytes of packet
            var byteData = packet.Compose();
            // Begin sending data
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }
        protected void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.  
                SendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        protected void Close()
        {
            ConsoleHelper.WriteLine("Server is closing", true);
            Running = false;
        }
    }
}
