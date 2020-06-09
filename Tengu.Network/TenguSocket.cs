using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Tengu.Network.Events;

namespace Tengu.Network
{
    public class TenguSocket : ISocket
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
            client.WorkSocket = handler;
            client.Packet.RawValues = new byte[ClientState.BufferSize];

            handler.BeginReceive(client.buffer, 0, ClientState.BufferSize,
            0, new AsyncCallback(EndReceive), client);

            InvokeOnClientConnect(client);
        }

        public void EndReceive(IAsyncResult ar)
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
                handler.BeginReceive(client.buffer, 0, ClientState.BufferSize, 0, new AsyncCallback(EndReceive), client);
            }
        }

        public void BeginConnect()
        {
            throw new NotImplementedException();
        }

        public void EndConnect(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        public void BeginSend()
        {
            throw new NotImplementedException();
        }

        public void EndSend()
        {
            throw new NotImplementedException();
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
