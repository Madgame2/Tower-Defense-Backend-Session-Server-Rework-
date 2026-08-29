using GameServer.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace GameServer.Application.Interfaces
{
    public interface IWsContextFactory
    {
        WsContext Create(WebSocket socket, string playerId, Guid sessionId);
    }
}
