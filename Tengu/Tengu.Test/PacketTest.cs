using System;
using Xunit;
using Xunit.Abstractions;
using Tengu.Network;

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
        public void StringsTest()
        {
            Packet outBound = new Packet();
            string usernameIn = "Leroy Jenkins";
            string passwordIn = "WelpsUnlimited";
            outBound.AddStringUTF8(usernameIn);
            outBound.AddStringUTF8(passwordIn);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            var values = inBound.Read();
            Assert.Equal(usernameIn, values[0]);
            Assert.Equal(passwordIn, values[1]);
        }
        [Fact]
        public void ShortTest()
        {
            Packet outBound = new Packet();
            short num1Out = 32;
            short num2Out = 1;
            short num3Out = 32767;
            outBound.AddValue(num1Out);
            outBound.AddValue(num2Out);
            outBound.AddValue(num3Out);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            var values = inBound.Read();
            Assert.Equal(num1Out, values[0]);
            Assert.Equal(num2Out, values[1]);
            Assert.Equal(num3Out, values[2]);
        }
        [Fact]
        public void IntTest()
        {
            Packet outBound = new Packet();
            int num1Out = 45;
            int num2Out = 128;
            int num3Out = 4398;
            outBound.AddValue(num1Out);
            outBound.AddValue(num2Out);
            outBound.AddValue(num3Out);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            var values = inBound.Read();
            Assert.Equal(num1Out, values[0]);
            Assert.Equal(num2Out, values[1]);
            Assert.Equal(num3Out, values[2]);
        }
        [Fact]
        public void LongTest()
        {
            Packet outBound = new Packet();
            long num1Out = 13;
            long num2Out = -426;
            long num3Out = 2147483647;
            outBound.AddValue(num1Out);
            outBound.AddValue(num2Out);
            outBound.AddValue(num3Out);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            var values = inBound.Read();
            Assert.Equal(num1Out, values[0]);
            Assert.Equal(num2Out, values[1]);
            Assert.Equal(num3Out, values[2]);
        }
        [Fact]
        public void FloatTest()
        {
            Packet outBound = new Packet();
            float num1Out = 145f;
            float num2Out = -1.5f;
            float num3Out = 3.456789834345f;
            outBound.AddValue(num1Out);
            outBound.AddValue(num2Out);
            outBound.AddValue(num3Out);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            var values = inBound.Read();
            Assert.Equal(num1Out, values[0]);
            Assert.Equal(num2Out, values[1]);
            Assert.Equal(num3Out, values[2]);
        }
        [Fact]
        public void DoubleTest()
        {
            Packet outBound = new Packet();
            double num1Out = 0.00000000000000000001;
            double num2Out = -6.577789;
            double num3Out = 1256.09078;
            outBound.AddValue(num1Out);
            outBound.AddValue(num2Out);
            outBound.AddValue(num3Out);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            var values = inBound.Read();
            Assert.Equal(num1Out, values[0]);
            Assert.Equal(num2Out, values[1]);
            Assert.Equal(num3Out, values[2]);
        }
        [Fact]
        public void MixTest()
        {
            Packet outBound = new Packet();
            int num1Out = 17;
            string text1Out = "Hello World!";
            long num2Out = 5857537345845834;
            float num3Out = 4.554545f;
            double num4Out = 1.123213123123123;
            string text2Out = "Inigo Montoya";

            outBound.AddValue(num1Out);
            outBound.AddStringUTF8(text1Out);
            outBound.AddValue(num2Out);
            outBound.AddValue(num3Out);
            outBound.AddValue(num4Out);
            outBound.AddStringUTF8(text2Out);

            Packet inBound = new Packet
            {
                Body = outBound.Compose()
            };

            inBound.ReadHeader();

            var values = inBound.Read();

            Assert.Equal(num1Out, values[0]);
            Assert.Equal(text1Out, values[1]);
            Assert.Equal(num2Out, values[2]);
            Assert.Equal(num3Out, values[3]);
            Assert.Equal(num4Out, values[4]);
            Assert.Equal(text2Out, values[5]);
        }
    }
}
