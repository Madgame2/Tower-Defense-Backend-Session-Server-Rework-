using GameServer.Contracts.MessageFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Model
{
    public readonly struct OutgoingMessage
    {
        public byte[] Buffer { get; }

        public int Length { get; }

        public bool ReturnToPool { get; }
        public MessageFormat MessageFormat { get; }

        public OutgoingMessage(byte[] buffer, int length, bool returnToPool, MessageFormat messageFormat)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Length = length;
            ReturnToPool = returnToPool;
            MessageFormat = messageFormat;
        }
    }
}
