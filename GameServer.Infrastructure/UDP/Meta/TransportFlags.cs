using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.UDP.Meta
{
    [Flags]
    internal enum TransportFlags : byte
    {
        None = 0,
        Reliable = 1 << 0,
        Acknowledgement = 1 << 1,
    }
}
