using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.UDP.Core
{
    public ref struct PacketReader
    {
        private ReadOnlySpan<byte> _data;
        private int _position;

        public PacketReader(ReadOnlySpan<byte> data)
        {
            _data = data;
            _position = 0;
        }

        public byte ReadByte()
        {
            return _data[_position++];
        }

        public uint ReadUInt32()
        {
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(
                _data.Slice(_position, 4));

            _position += 4;

            return value;
        }
        public int ReadInt32()
        {
            int value = BinaryPrimitives.ReadInt32LittleEndian(
                _data.Slice(_position, 4));

            _position += 4;

            return value;
        }

        public float ReadSingle()
        {
            int value = BinaryPrimitives.ReadInt32LittleEndian(
                _data.Slice(_position, 4));

            _position += 4;

            return BitConverter.Int32BitsToSingle(value);
        }
    }
}
