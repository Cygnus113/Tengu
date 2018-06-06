using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Tengu.Network
{
    public class WebSocketServer : TcpServer
    {
        public WebSocketServer(string Tag = "Tengu WebSocket") : base (Tag) { }

        protected override void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            AllDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the ClientState object and begin recieving data 
            ClientState client = new ClientState();
            client.WorkSocket = handler;

            // Recieve the upgrade request in an async recieve callback
            handler.BeginReceive(client.buffer, 0, ClientState.BufferSize,
            0, new AsyncCallback(Handshake), client);

            OnClientConnect(client);
        }
        protected void Handshake(IAsyncResult ar)
        {
            // TODO: WebSocket handshake protocol

            // Get ClientState and Socket from Async Result
            ClientState client = (ClientState)ar.AsyncState;
            Socket handler = client.WorkSocket;

            // Point torwards regular Readcallback after handshake
            handler.BeginReceive(client.buffer, 0, ClientState.BufferSize,
            0, new AsyncCallback(ReadCallback), client);
        }
    }
}
