using System;
using System.Collections.Generic;
using System.Text;

namespace Tengu.Network
{
    public partial class Packet
    {
        public void AddValue(int value)
        {
            byte[] header = new byte[] { (int)DataType.Int32 };
            byte[] numBytes = BitConverter.GetBytes(value);

            Values.Add(header);
            Values.Add(numBytes);
        }
    }
}
