using System;
using System.Collections.Generic;
using System.Text;

namespace Tengu.Network
{
    public partial class Packet
    {
        public void Read()
        {
            List<object> values = new List<object>();

            while (_readIndex <= Length - 1)
            {
                values.Add(ReadValue());
            }

            Values = values;
        }

        private object ReadValue()
        {
            DataType valueType = (DataType)RawValues[_readIndex];
            _readIndex += 1;
            object value;
            switch (valueType)
            {
                case DataType.Int16:
                    value = ReadShort();
                    break;
                case DataType.Int32:
                    value = ReadInt();
                    break;
                case DataType.UTF8:
                    value = ReadStringUTF8();
                    break;
                default:
                    throw new ArgumentException("DataType not supported");
            }

            return value;
        }

        private string ReadStringUTF8()
        {
            short stringLength = ReadShort();
            byte[] stringBytes = new byte[stringLength];

            Buffer.BlockCopy(RawValues, _readIndex, stringBytes, 0, stringLength);
            _readIndex += stringLength;
            string data = Encoding.UTF8.GetString(stringBytes);

            return data;
        }
        private short ReadShort()
        {
            short number = BitConverter.ToInt16(RawValues, _readIndex);
            _readIndex += 2;
            return number;
        }
        private int ReadInt()
        {
            int number = BitConverter.ToInt32(RawValues, _readIndex);
            _readIndex += 4;
            return number;
        }
    }
}
