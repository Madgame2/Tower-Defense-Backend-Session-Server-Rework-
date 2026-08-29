using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface IPacketRouter
    {
        void Route(GameRoom room, string playerId, uint udpToken, byte packetType, ReadOnlySpan<byte> payload);
    }
}
