using NUnit.Framework;
using System;
using System.Net.Sockets;
using Tengu.Network;

namespace Tengu.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DataTypesToHex()
        {
            var packet = new Packet();
            packet.AddValue(3756);
            var bytes = packet.BuildPacketBody();

            var hex = BitConverter.ToString(bytes).Replace("-", "");
            Console.WriteLine(hex);
        }

    }
}