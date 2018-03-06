using System;
using System.Collections.Generic;
using System.Text;

namespace Tengu.Network
{
    public static class Header
    {
        public abstract class Utility
        {
            public const short Base = 1;
            public const short Heartbeat = 1;
            public const short Connect = 2;
            public const short Disconnect = 3;
        }

        public abstract class Login
        {
            public const short Base = 1;
            public const short Credentials = 1;
        }
    }
}
