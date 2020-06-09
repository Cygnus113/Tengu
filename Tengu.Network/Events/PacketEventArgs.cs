using System;
using System.Collections.Generic;
using System.Text;

namespace Tengu.Network.Events
{
    public class PacketEventArgs
    {
        public Packet Packet { get; set; }
        public ClientState Client { get; set; }
    }
}
