using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Tengu.Network
{
    public interface ISocket
    {
        void BeginAccept(int localPort, string localAddressString, ProtocolType socketProtoco);
        void EndAccept(IAsyncResult ar);
        void EndReceive(IAsyncResult ar);

        void BeginConnect();
        void EndConnect(IAsyncResult ar);

    }
}
