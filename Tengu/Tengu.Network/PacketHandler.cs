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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="BaseID"></param>
        /// <param name="SubID"></param>
        /// <param name="method"></param>
        protected void RegisterAction(short baseID, short subID, Action<Packet> method)
        {
            Tuple<short, short> key = Tuple.Create(baseID, subID);
            ActionDictionary.Add(key, method);
        }
        /// <summary>
        /// Invoke the method of a given packet
        /// </summary>
        /// <param name="packet"></param>
        public void Invoke(Packet packet)
        {
            GetAction(packet).Invoke();
        }
        private Action GetAction(Packet packet)
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
