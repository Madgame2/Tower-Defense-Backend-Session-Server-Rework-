using GameServer.Contracts.UDP.Contract;
using GameServer.Contracts.UDP.Enums;
using GameServer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Contracts.UDP.Packets
{
    public readonly ref struct ClientInputPacket : IUdpPacket
    {
        public uint UdpToken { get; }
        public PacketType Type => PacketType.MoveInput;
        public uint LastTick { get; }
        public ReadOnlySpan<MoveInputCommand> Inputs { get; }

        public ClientInputPacket(uint udpToken, uint lastTick, ReadOnlySpan<MoveInputCommand> inputs)
        {
            UdpToken = udpToken;
            LastTick = lastTick;
            Inputs = inputs;
        }

        public void Serialize(BinaryWriter writer)
        {
            // 1. Сначала пишем заголовок (5 байт)
            writer.Write(UdpToken);    // 4 байта
            writer.Write((byte)Type);  // 1 байт

            // 2. Затем пишем payload (ваши данные)
            writer.Write(LastTick);    // 4 байта
            writer.Write(Inputs.Length); // 4 байта

            foreach (ref readonly var input in Inputs)
            {
                writer.Write(input.Tick);
                writer.Write(input.MoveDirection.X);
                writer.Write(input.MoveDirection.Y);
                writer.Write(input.MoveDirection.Z);
            }
        }
    }
}
