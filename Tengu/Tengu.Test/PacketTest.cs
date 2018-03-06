using System;
using Tengu.Network;
using Xunit;
using Xunit.Abstractions;

namespace Tengu.Test
{
    public class PacketTest
    {
        private readonly ITestOutputHelper Output;

        public PacketTest(ITestOutputHelper Output)
        {
            this.Output = Output;
        }
        [Fact]
        public void IDTest()
        {
            Packet outBound = new Packet()
            {
                BaseID = 1,
                SubID = 2,
            };

            byte[] packetBytes = outBound.Compose();

            Packet inBound = new Packet()
            {
                Body = packetBytes
            };

            inBound.ReadHeader();

            Assert.Equal(outBound.BaseID, inBound.BaseID);
            Assert.Equal(outBound.SubID, inBound.SubID);
        }
        [Fact]
        public void StringTest()
        {
            Packet outBound = new Packet();
            string initialString = "Hello World";
            outBound.AddStringUTF8(initialString);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            string assertString = inBound.ReadStringUTF8();

            Assert.Matches(initialString, assertString);
            Output.WriteLine(assertString);
        }
        [Fact]
        public void DoubleStringTest()
        {
            Packet OutBound = new Packet()
            {
                BaseID = 32,
                SubID = 5
            };

            var UsernameOut = "Leroy Jenkins";
            var PasswordOut = "My Password";
            OutBound.AddStringUTF8(UsernameOut);
            OutBound.AddStringUTF8(PasswordOut);

            Packet InBound = new Packet()
            {
                Body = OutBound.Compose()
            };

            InBound.ReadHeader();
            var UsernameIn = InBound.ReadStringUTF8();
            var PasswordIn = InBound.ReadStringUTF8();

            Output.WriteLine(UsernameOut);
            Output.WriteLine(PasswordOut);
            Assert.Matches(UsernameIn, UsernameOut);
            Assert.Matches(PasswordIn, PasswordOut);
        }
        [Fact]
        public void IntTest()
        {
            Packet outBound = new Packet()
            {
                BaseID = 54,
                SubID = 31,
            };

            var outInt = 3400;
            outBound.AddInt(outInt);

            byte[] packetBytes = outBound.Compose();

            Packet inBound = new Packet()
            {
                Body = packetBytes
            };

            inBound.ReadHeader();
            int inInt = inBound.ReadInt();

            Assert.Equal(outInt, inInt);
        }

        public void HandleTest()
        {
            Packet p = new Packet
            {
                BaseID = Header.Utility.Base,
                SubID = Header.Utility.Heartbeat
            };

            PacketHandler PH = new PacketHandler();
            var act = PH.GetAction(p);

            Assert.NotNull(act);
        }
    }
}
