using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tengu.Network
{
    public partial class Packet
    {
        public int? Length { get; private set; }

        public byte[] RawValues { get; set; }

        public List<object> Values { get; private set; }

        private int _readIndex = 0;

        public Packet()
        {
            Values = new List<object>();
        }

        internal void SetLength(byte[] length)
        {
            if (!Length.HasValue)
            {
                Length = BitConverter.ToInt32(length, 0);
                _readIndex += 4;
            }
        }

        public byte[] BuildPacketBody()
        {
            // Allocate byte[] for the actual length of the data
            int dataLength = 0;
            foreach (byte[] byteArray in Values)
            {
                dataLength += byteArray.Length;
            }

            // Add packet length to begginning of data
            var totalLength = (dataLength + 4);
            byte[] body = new byte[totalLength];
            var lengthBits = BitConverter.GetBytes(totalLength);
            Buffer.BlockCopy(lengthBits, 0, body, 0, lengthBits.Length);

            // Copy each byte[] to body
            int index = 4;
            foreach (byte[] byteArray in Values)
            {
                Buffer.BlockCopy(byteArray, 0, body, index, byteArray.Length);
                index += byteArray.Length;
            }

            return body;
        }
    }
}
