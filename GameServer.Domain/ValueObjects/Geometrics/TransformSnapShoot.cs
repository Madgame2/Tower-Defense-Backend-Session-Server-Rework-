using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.ValueObjects.Geometrics
{
    using System;
    using System.Buffers.Binary;
    using System.Numerics;

    public struct TransformSnapShoot
    {
        public const int ByteSize = 40;

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public int WriteTo(Span<byte> destination)
        {
            if (destination.Length < ByteSize)
                throw new ArgumentException("Destination buffer is too small.");

            int offset = 0;

            WriteVector3(destination, ref offset, Position);
            WriteQuaternion(destination, ref offset, Rotation);
            WriteVector3(destination, ref offset, Scale);

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
