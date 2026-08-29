using GameServer.Application.Interfaces;
using GameServer.Application.Model;
using GameServer.Contracts.interfaces;
using GameServer.Contracts.MessageFormats;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.Networking
{
    public class SessionBroadcaster : ISessionBroadcaster
    {
        private readonly IConnectedClientsStorage _clientsStorage;
        private readonly IMessageSerializer _serializer;

        public SessionBroadcaster(IConnectedClientsStorage clientsStorage, IMessageSerializer serializer)
        {
            _clientsStorage = clientsStorage;
            _serializer = serializer;
        }

        public void SendToPlayer<T>(string userId, T message, MessageFormat messageFormat) where T : IServerMessage
        {
            if (!_clientsStorage.TryGet(userId, out var connection)) return;

            var (buffer, length) = _serializer.SerializePooled(message, messageFormat);

            connection.SendPooled(buffer, length, messageFormat);
        }

        public void BroadcastToSession<T>(IEnumerable<string> playerIds, T message, MessageFormat messageFormat) where T : IServerMessage
        {
            byte[] broadcastBuffer = _serializer.SerializeToArray(message, messageFormat);
            int length = broadcastBuffer.Length;

            foreach (var userId in playerIds)
            {
                if (_clientsStorage.TryGet(userId, out var connection))
                {
                    connection.SendShared(broadcastBuffer, length, messageFormat);
                }
            }
        }

        public void SendToPlayer<T>(ClientConnection clientObject, T message, MessageFormat messageFormat) where T : IServerMessage
        {
            var (buffer, length) = _serializer.SerializePooled(message, messageFormat);

            clientObject.SendPooled(buffer, length, messageFormat);
        }
    }
}
