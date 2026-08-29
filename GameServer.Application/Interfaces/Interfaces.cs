using GameServer.Application.Model;
using GameServer.Contracts.interfaces;
using GameServer.Contracts.MessageFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Interfaces
{
    public interface ISessionBroadcaster
    {
        void SendToPlayer<T>(string userId, T message, MessageFormat messageFormat) where T : IServerMessage;
        void SendToPlayer<T>(ClientConnection clientObject, T message, MessageFormat messageFormat) where T : IServerMessage;

        void BroadcastToSession<T>(IEnumerable<string> playerIds, T message, MessageFormat messageFormat) where T : IServerMessage;
    }
}
