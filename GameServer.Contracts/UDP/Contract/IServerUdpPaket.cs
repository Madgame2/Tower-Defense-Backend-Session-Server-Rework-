using GameServer.Contracts.UDP.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.UDP.Contract
{
    public interface IServerUdpPaket
    {
        PacketType Type { get; }

        int Serialize(Span<byte> buffer);
    }
}
