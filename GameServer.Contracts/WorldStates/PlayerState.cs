using GameServer.Contracts.UDP.Enums;
using GameServer.Domain.Player;
using GameServer.Domain.Player.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Contracts.WorldStates
{
    public struct PlayerState
    {
        public uint ObjectId;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Lookat;
        public Vector3 Velocity;
        public MovementState MovementState;

        public const int SerializedSize = 57;

        public PlayerState(uint objectId, Vector3 position, Quaternion rotation, Vector3 lookat, Vector3 velocity, MovementState movementState)
        {
            ObjectId = objectId;
            Position = position;
            Rotation = rotation;
            Lookat  = lookat;
            Velocity = velocity;
            MovementState = movementState;
        }

        public int Serialize(Span<byte> buffer)
        {
            if (buffer.Length < SerializedSize)
                return 0;

            MemoryMarshal.Write(buffer.Slice(0), in ObjectId);
            MemoryMarshal.Write(buffer.Slice(4), in Position);
            MemoryMarshal.Write(buffer.Slice(16), in Rotation);
            MemoryMarshal.Write(buffer.Slice(32), in Lookat);
            MemoryMarshal.Write(buffer.Slice(44), in Velocity);

            buffer[56] = (byte)MovementState;

            return SerializedSize;
        }
    }
}
