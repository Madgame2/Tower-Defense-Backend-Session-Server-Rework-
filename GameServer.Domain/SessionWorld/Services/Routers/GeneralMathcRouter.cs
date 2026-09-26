using GameServer.Contracts.UDP.Enums;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Infrastructure.UDP.Deserializer;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Domain.SessionWorld.Routers
{
    public class GeneralMathcRouter : IPacketRouter
    {
        public void Route(GameRoom room, string playerId, uint udpToken, byte packetType, ReadOnlySpan<byte> payload)
        {
            var type = (PacketType)packetType;

            switch (type)
            {
                case PacketType.MoveInput:

                    if (UdpPacketDeserializer.TryDeserializeInput(payload, udpToken, out var movePacket))
                    {
                        room.HandleMoveInputs(playerId, movePacket.LastTick, movePacket.Inputs);
                    }
                    break;
            }
        }
    }
}
