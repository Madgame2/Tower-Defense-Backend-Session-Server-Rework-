using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload
{

    public struct ProjectilePayload
    {
        public uint prefabId;
        public Vector3 Position;
        public Quaternion Rotation;

        public const int Size = 32;

        internal int WriteTo(Span<byte> destination)
        {
            if (destination.Length < Size)
                throw new ArgumentException("Destination buffer is too small.");

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination,
                prefabId);

            int offset = 4;

            WriteVector3(destination, ref offset, Position);

            WriteQuaternion(destination, ref offset, Rotation);

            return offset;
        }

        private static void WriteVector3(
            Span<byte> destination,
            ref int offset,
            Vector3 value)
        {
            WriteFloat(destination, ref offset, value.X);
            WriteFloat(destination, ref offset, value.Y);
            WriteFloat(destination, ref offset, value.Z);
        }

        private static void WriteQuaternion(
            Span<byte> destination,
            ref int offset,
            Quaternion value)
        {
            WriteFloat(destination, ref offset, value.X);
            WriteFloat(destination, ref offset, value.Y);
            WriteFloat(destination, ref offset, value.Z);
            WriteFloat(destination, ref offset, value.W);
        }

        private static void WriteFloat(
            Span<byte> destination,
            ref int offset,
            float value)
        {
            BinaryPrimitives.WriteInt32LittleEndian(
                destination.Slice(offset, 4),
                BitConverter.SingleToInt32Bits(value));

            offset += 4;
        }
    }
}
