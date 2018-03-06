using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Tengu.Network
{
    // A class used to encapsulate network data
    public class Packet
    {
        public short BaseID { get; set; }
        public short SubID { get; set; }
        public short Length = 0;
        public byte[] Body { get; set; }

        public int BufferLength = 1024;
        public int ReadIndex;
        public List<byte[]> BodyBuilder { get; set; }
        public Socket Source { get; set; }

        public Packet()
        {
            // Arbitrary value to accommodate incoming data
            Body = new byte[BufferLength];
            BodyBuilder = new List<byte[]>();
            ReadIndex = 0;
        }
        public byte[] Compose()
        {
            // Get the bytes of the packet IDs 
            byte[] baseBytes = BitConverter.GetBytes(BaseID);
            byte[] subBytes = BitConverter.GetBytes(SubID);

            // Get byte[] of all data added to packet
            Body = ConcatenateBody();

            // Get total length of packet
            short totalLength = (short)(6 + Body.Length);

            // Allocate byte[] for the packet data
            byte[] packetBytes = new byte[totalLength];
            // Get bytes of Packet Length short
            byte[] lengthBytes = BitConverter.GetBytes(totalLength);

            // Copy data into outgoing packetByte
            Buffer.BlockCopy(baseBytes, 0, packetBytes, 0, 2);
            Buffer.BlockCopy(subBytes, 0, packetBytes, 2, 2);
            Buffer.BlockCopy(lengthBytes, 0, packetBytes, 4, 2);
            Buffer.BlockCopy(Body, 0, packetBytes, 6, Body.Length);

            return packetBytes;
        }
        public byte[] ConcatenateBody()
        {
            // Allocate byte[] for the actual length of the data
            int len = 0;
            foreach (var byteArray in BodyBuilder)
            {
                len += byteArray.Length;
            }
            byte[] body = new byte[len];

            // Copy each byte[] to body
            int index = 0;
            foreach (var byteArray in BodyBuilder)
            {
                Buffer.BlockCopy(byteArray, 0, body, index, byteArray.Length);
                index += byteArray.Length;
            }

            return body;
        }

        public void ReadHeader()
        {
            BaseID = ReadShort();
            SubID = ReadShort();
            Length = ReadShort();
        }
        public void AddStringUTF8(string data)
        {
            // Add a UTF8 string and its length to the BodyBuilder
            byte[] stringData = Encoding.UTF8.GetBytes(data);

            BodyBuilder.Add(BitConverter.GetBytes((short)stringData.Length));
            BodyBuilder.Add(stringData);
        }
        public string ReadStringUTF8()
        {
            // Get string length
            short stringLength = ReadShort();
            byte[] stringBytes = new byte[stringLength];

            // Get decoded string
            Buffer.BlockCopy(Body, ReadIndex, stringBytes, 0, stringLength);
            ReadIndex += stringLength;
            string data = Encoding.UTF8.GetString(stringBytes);

            return data;
        }
        public void AddShort(short data)
        {
            BodyBuilder.Add(BitConverter.GetBytes(data));
        }
        public short ReadShort()
        {
            short number = BitConverter.ToInt16(Body, ReadIndex);
            ReadIndex += 2;
            return number;
        }
        public void AddInt(int data)
        {
            BodyBuilder.Add(BitConverter.GetBytes(data));
        }
        public int ReadInt()
        {
            int number = BitConverter.ToInt32(Body, ReadIndex);
            ReadIndex += 4;
            return number;
        }
        public void AddDouble(double data)
        {
            BodyBuilder.Add(BitConverter.GetBytes(data));
        }
        public double ReadDouble()
        {
            double number = BitConverter.ToInt32(Body, ReadIndex);
            ReadIndex += 8;
            return number;
        }
        public void AddFloat(float data)
        {
            BodyBuilder.Add(BitConverter.GetBytes(data));
        }
        public float ReadFloat()
        {
            float number = BitConverter.ToSingle(Body, ReadIndex);
            ReadIndex += 4;
            return number;
        }
    }
}
