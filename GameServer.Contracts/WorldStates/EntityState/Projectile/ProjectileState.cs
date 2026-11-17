using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Contracts.WorldStates.EntityState.Projectile
{
    public struct ProjectileState
    {
        // uint (4) + Vector3 (3 * 4) + Quaternion (4 * 4)
        public const int Size = sizeof(uint) + 3 * sizeof(float) + 4 * sizeof(float);

        public uint ObjectId;
        public Vector3 Position;
        public Quaternion Rotation;

        public readonly int GetSize() => Size;

        public readonly int WriteTo(Span<byte> destination)
        {
            if (destination.Length < Size)
                throw new ArgumentException(
                    $"Буфер слишком мал: нужно {Size} байт, передано {destination.Length}.",
                    nameof(destination));

            var offset = 0;

            WriteUInt32(destination, ref offset, ObjectId);
            WriteVector3(destination, ref offset, Position);
            WriteQuaternion(destination, ref offset, Rotation);

            return offset; // == Size
        }

        public static ProjectileState ReadFrom(ReadOnlySpan<byte> source)
        {
            if (source.Length < Size)
                throw new ArgumentException(
                    $"Буфер слишком мал: нужно {Size} байт, передано {source.Length}.",
                    nameof(source));

            var offset = 0;

            return new ProjectileState
            {
                ObjectId = ReadUInt32(source, ref offset),
                Position = ReadVector3(source, ref offset),
                Rotation = ReadQuaternion(source, ref offset)
            };
        }

        // ---- Write helpers ----

        private static void WriteUInt32(Span<byte> buffer, ref int offset, uint value)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(offset), value);
            offset += sizeof(uint);
        }

        private static void WriteSingle(Span<byte> buffer, ref int offset, float value)
        {
            BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset), value);
            offset += sizeof(float);
        }

        private static void WriteVector3(Span<byte> buffer, ref int offset, Vector3 v)
        {
            WriteSingle(buffer, ref offset, v.X);
            WriteSingle(buffer, ref offset, v.Y);
            WriteSingle(buffer, ref offset, v.Z);
        }

        private static void WriteQuaternion(Span<byte> buffer, ref int offset, Quaternion q)
        {
            WriteSingle(buffer, ref offset, q.X);
            WriteSingle(buffer, ref offset, q.Y);
            WriteSingle(buffer, ref offset, q.Z);
            WriteSingle(buffer, ref offset, q.W);
        }

        // ---- Read helpers ----

        private static uint ReadUInt32(ReadOnlySpan<byte> buffer, ref int offset)
        {
            var value = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(offset));
            offset += sizeof(uint);
            return value;
        }

        private static float ReadSingle(ReadOnlySpan<byte> buffer, ref int offset)
        {
            var value = BinaryPrimitives.ReadSingleLittleEndian(buffer.Slice(offset));
            offset += sizeof(float);
            return value;
        }

        private static Vector3 ReadVector3(ReadOnlySpan<byte> buffer, ref int offset) =>
            new(ReadSingle(buffer, ref offset),
                ReadSingle(buffer, ref offset),
                ReadSingle(buffer, ref offset));

        private static Quaternion ReadQuaternion(ReadOnlySpan<byte> buffer, ref int offset) =>
            new(ReadSingle(buffer, ref offset),
                ReadSingle(buffer, ref offset),
                ReadSingle(buffer, ref offset),
                ReadSingle(buffer, ref offset));
    }
}
