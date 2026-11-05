using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models
{
    public readonly struct ReplicationLogEntry
    {
        public const int HeaderSize = 4;

        public uint Tick { get; }
        public ServerEvent Event { get; }

        public ReplicationLogEntry(uint tick, ServerEvent evt)
        {
            Tick = tick;
            Event = evt;
        }

        public int GetSize()
        {
            return HeaderSize + Event.GetSize();
        }

        public int WriteTo(Span<byte> destination)
        {
            int size = GetSize();

            if (destination.Length < size)
                throw new ArgumentException("Destination buffer is too small.");

            BinaryPrimitives.WriteUInt32LittleEndian(
                destination.Slice(0, 4), Tick);

            return HeaderSize + Event.WriteTo(destination.Slice(HeaderSize));
        }
    }
}
