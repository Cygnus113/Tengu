using System;
using System.Collections.Generic;
using System.Text;
using Tengu.Network;

namespace Tengu.Integration
{
    public class WebSocketServer : TcpServer
    {
        //private LoginHandler PacketHandler;

        int LocalPort = 1400;
        string LocalAddress = "127.0.0.1";


        public WebSocketServer() : base("Websocket Server")
        {
            //PacketHandler = new LoginHandler(this);
            UseWebSockets();
            StartServer(LocalPort, LocalAddress);
        }
        override protected void HandleMessage(Packet packet)
        {
            //PacketHandler.GetAction(packet).Invoke();
        }

        protected override void OnClientConnect(ClientState client)
        {
            Console.WriteLine("A client connected");
        }
    }
}
