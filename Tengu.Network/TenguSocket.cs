using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using Tengu.Network.Events;

namespace Tengu.Network
{
    public class TenguSocket
    {
        public bool Running { get; private set; } = false;

        private readonly ILogger _logger; 
        private readonly byte _packetLengthTypeLen = 4;

        // Event Signals for async threads
        private static ManualResetEvent _allDone = new ManualResetEvent(false);
        private static ManualResetEvent _connectDone = new ManualResetEvent(false);
        private static ManualResetEvent _sendDone = new ManualResetEvent(false);

        public event EventHandler<PacketEventArgs> OnMessage;
        public event EventHandler<PacketEventArgs> OnClientConnect;

        public TenguSocket(ILogger logger)
        {
            this._logger = logger;
        }

        public void BeginAccept(int localPort, string localAddressString, ProtocolType socketProtocol)
        {
            IPAddress localAddress = null;

            try
            {
                localAddress = IPAddress.Parse(localAddressString);
            }
            catch (Exception)
            {
                _logger.LogError($"Cannot start Server: Invalid IP Address: {localAddressString}");
                throw;
            }

            // Establish local endpoint
            IPEndPoint localEndPoint = new IPEndPoint(localAddress, localPort);

            // Create a socket.  
            Socket listener = new Socket(localAddress.AddressFamily,
                SocketType.Stream, socketProtocol);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                // Accept Clients continuously
                Running = true;
                while (Running)
                {
                    // Set the event to nonsignaled state.  
                    _allDone.Reset();
                    _logger.LogInformation("Listening for connections...");

                    // Start listening and bind callback method  
                    listener.BeginAccept(
                        new AsyncCallback(EndAccept),
                        listener);

                    // Wait until a connection is made before continuing.  
                    _allDone.WaitOne();
                }
            }
            catch (Exception)
            {
                _logger.LogError("An error occured in the server");
                throw;
            }
        }
        public virtual void EndAccept(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            _allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the ClientState object and begin recieving data 
            ClientState client = new ClientState();
            client.Packet.RawValues = new byte[ClientState.BufferSize];
            client.WorkSocket = handler;

            InvokeOnClientConnect(client);
            BeginRead(client);
        }
        public void BeginRead(ClientState client)
        {
            client.WorkSocket.BeginReceive(client.buffer, 0, ClientState.BufferSize,
            0, new AsyncCallback(EndRead), client);
        }
        public void EndRead(IAsyncResult ar)
        {
            // Get ClientState and Socket from Async Result
            ClientState client = (ClientState)ar.AsyncState;
            Socket handler = client.WorkSocket;

            // Amount of read data from the client socket.  
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead >= _packetLengthTypeLen)
            {
                client.Packet.SetLength(new byte[] { client.buffer[0], client.buffer[1], client.buffer[2], client.buffer[3] });

                // If all the data has been read, handle the message
                if (bytesRead >= client.Packet.Length)
                {
                    // Copy data from client buffer to packet body
                    client.Packet.RawValues = client.buffer;
                    InvokeOnMessage(client.Packet, client);
                }
            }

            // Continue recieving data again from this client
            if (handler.Connected)
            {
                handler.BeginReceive(client.buffer, 0, ClientState.BufferSize, 0, new AsyncCallback(EndRead), client);
            }
        }
        public void BeginConnect(string ip, int serverPort, ref Socket outSocket)
        {
            // Get IP Address
            IPAddress ipAddress = IPAddress.Parse(ip.Trim());
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, serverPort);

            // Save the socket for user reference later  
            outSocket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            outSocket.BeginConnect(remoteEP,
                new AsyncCallback(EndConnect), outSocket);
            _connectDone.WaitOne();
        }
        internal void EndConnect(IAsyncResult ar)
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
                _connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        internal void BeginWrite(Socket client, Packet packet)
        {
            // Get bytes of packet
            var byteData = packet.BuildPacketBody();
            // Begin sending data
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(EndWrite), client);
        }
        internal void BeginWrite(Socket client, byte[] byteData)
        {
            // Begin sending data
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(EndWrite), client);
        }
        internal void EndWrite(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.  
                _sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void InvokeOnMessage(Packet packet, ClientState client)
        {
            PacketEventArgs args = new PacketEventArgs();
            args.Packet = packet;
            args.Client = client;
            OnMessage.Invoke(this, args);
        }
        private void InvokeOnClientConnect(ClientState client)
        {
            PacketEventArgs args = new PacketEventArgs();
            args.Client = client;
            OnClientConnect.Invoke(this, args);
        }
    }
}
