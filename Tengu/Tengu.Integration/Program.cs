using System;

namespace Tengu.Integration
{
    public class Program
    {
        static void Main(string[] args)
        {
            WebSocketTestServer w = new WebSocketTestServer();
            w.StartServer(w.port, w.address);
        }
    }
}
