using GameServer.Contracts.UDP.Contract;
using GameServer.Contracts.UDP.Enums;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Contracts.UDP.Pakets
{
    public struct PlayerStateSnapshot : IServerUdpPaket
    {
        public PacketType Type => PacketType.PlayerWorldState;
        public string UserId;
        public uint ServerTick;
        public Vector3 Position;

        public PlayerStateSnapshot(string userId, uint serverTick, Vector3 position)
        {
            UserId = userId;
            Position = position;
            ServerTick = serverTick;
        }

        public int Serialize(Span<byte> buffer)
        {
            int offset = 0;

            buffer[offset] = (byte)Type;
            offset += 1;

            int stringByteLength = Encoding.UTF8.GetByteCount(UserId);

            ushort length = (ushort)stringByteLength;
            MemoryMarshal.Write(buffer.Slice(offset), ref length);
            offset += 2;

            Encoding.UTF8.GetBytes(UserId, buffer.Slice(offset));
            offset += stringByteLength;

            uint tick = ServerTick;
            MemoryMarshal.Write(buffer.Slice(offset), ref tick);
            offset += 4;

            float x = Position.X;
            float y = Position.Y;
            float z = Position.Z;

            MemoryMarshal.Write(buffer.Slice(offset), ref x);
            offset += 4;

            MemoryMarshal.Write(buffer.Slice(offset), ref y);
            offset += 4;

            MemoryMarshal.Write(buffer.Slice(offset), ref z);
            offset += 4;

            return offset;
        }
    }
}
