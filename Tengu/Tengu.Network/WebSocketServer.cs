using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Tengu.Network
{
    public class WebSocketServer : TcpServer
    {
        public WebSocketServer(string Tag = "Tengu WebSocket") : base (Tag) { }
        private const string Guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        private int WebSocketVersion = 13;

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

            var request = Encoding.UTF8.GetString(client.buffer);

            Regex webSocketKeyRegex = new Regex("Sec-WebSocket-Key: (.*)");
            Regex webSocketVersionRegex = new Regex("Sec-WebSocket-Version: (.*)");

            int secWebSocketVersion = Convert.ToInt32(webSocketVersionRegex.Match(request).Groups[1].Value.Trim());
            string secWebSocketKey = webSocketKeyRegex.Match(request).Groups[1].Value.Trim();

             //TODO: Check version

            var computedKey = ComputeKey(secWebSocketKey);
            var response =  Encoding.UTF8.GetBytes(
                           "HTTP/1.1 101 Switching Protocols\r\n"
                         + "Connection: Upgrade\r\n"
                         + "Upgrade: websocket\r\n"
                         + "Sec-WebSocket-Accept: " + computedKey + "\r\n"
                         + "\r\n");

            Send(handler, response);

            // Point torwards regular Readcallback after handshake
            handler.BeginReceive(client.buffer, 0, ClientState.BufferSize,
            0, new AsyncCallback(ReadCallback), client);
        }

        private string ComputeKey(string key)
        {
            string keyGuid = key + Guid;
            SHA1 sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(keyGuid));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
