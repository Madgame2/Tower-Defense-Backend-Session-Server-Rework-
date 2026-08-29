using GameServer.Contracts.UDP.Packets;
using GameServer.Domain.ValueObjects;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Infrastructure.UDP.Deserializer
{
    internal static class UdpPacketDeserializer
    {
        public static bool TryDeserializeInput(ReadOnlySpan<byte> data, uint udpToken, out ClientInputPacket packet)
        {
            packet = default;

            if (data.Length < 8) return false;

            uint lastTick = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0, 4));
            int inputsCount = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(4, 4));

            if (inputsCount < 0) return false;

            int commandSize = Unsafe.SizeOf<MoveInputCommand>();
            int expectedPayloadSize = inputsCount * commandSize;

            if (data.Length < 8 + expectedPayloadSize) return false;

            ReadOnlySpan<byte> inputsBytes = data.Slice(8, expectedPayloadSize);

            ReadOnlySpan<MoveInputCommand> inputs = MemoryMarshal.Cast<byte, MoveInputCommand>(inputsBytes);

            packet = new ClientInputPacket(udpToken, lastTick, inputs);
            return true;
        }
    }
}
