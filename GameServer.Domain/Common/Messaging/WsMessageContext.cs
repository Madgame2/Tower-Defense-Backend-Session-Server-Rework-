using GameServer.Contracts.WsMessaging.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Messaging
{
    public struct WsMessageContext
    {
        public WsContext ConnectionContext { get; init; }
        public string? Text { get; set; }
        public ReadOnlyMemory<byte> Binary { get; set; }

        public WsMessage Message { get; set; }
    }
}
