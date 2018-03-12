using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


namespace Tengu.Network
{
    public class PacketHandler
    {
        protected Dictionary<Tuple<short, short>, Action<Packet>> ActionDictionary;

        public PacketHandler()
        {
            ActionDictionary = new Dictionary<Tuple<short, short>, Action<Packet>>();
        }
        protected void RegisterAction(short BaseID, short SubID, Action<Packet> method)
        {
            Tuple<short, short> key = Tuple.Create(BaseID, SubID);
            ActionDictionary.Add(key, method);
        }
        public void Invoke(Packet packet)
        {
            // Get and then invoke the action mapped to this packet
            GetAction(packet).Invoke();
        }
        public Action GetAction(Packet packet)
        {
            // Create the Tuple key and get the action
            Tuple<short, short> key = Tuple.Create(packet.BaseID, packet.SubID);
            Action<Packet> getAction;
            ActionDictionary.TryGetValue(key, out getAction);

            // Insert packet into action
            Action paramAction = () => getAction(packet);
            return paramAction;
        }
    }
}
