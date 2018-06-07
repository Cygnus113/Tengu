using System;
using System.Collections.Generic;
using System.Text;
using Tengu.Network;

namespace Tengu.Integration
{
    public class WebSocketTestServer : WebSocketServer
    {
        public int port = 1400;
        public string address = "127.0.0.1";

        public WebSocketTestServer() : base("Websocket Test")
        {

        }
    }
}
