using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.UDP.Meta
{
    public enum PacketDeliveryStatus : byte
    {
        Acknowledged,
        TimedOut,
        Cancelled,
        SendFailed,
        QueueFull,
        TransportDisposed
    }

    public readonly struct PacketDeliveryResult
    {
        public PacketDeliveryStatus Status { get; }
        public uint SequenceId { get; }
        public Exception Exception { get; }

        public bool IsAcknowledged =>
            Status == PacketDeliveryStatus.Acknowledged;

        public PacketDeliveryResult(
            uint sequenceId,
            PacketDeliveryStatus status,
            Exception exception = null)
        {
            SequenceId = sequenceId;
            Status = status;
            Exception = exception;
        }
    }
}
