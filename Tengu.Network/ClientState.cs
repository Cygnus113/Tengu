using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Tengu.Network
{
    public class ClientState
    {
        // Client Socket.  
        public Socket WorkSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Send / Recieve network data in this class
        public Packet Packet = new Packet();
    }
}
