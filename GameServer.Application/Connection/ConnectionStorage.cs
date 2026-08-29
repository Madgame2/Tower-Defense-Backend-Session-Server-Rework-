using GameServer.Application.Interfaces;
using GameServer.Application.Model;
using GameServer.Infrastructure.Exceptions.ClientsStorage;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace GameServer.Infrastructure.Connection
{

    public class ConnectionStorage : IConnectedClientsStorage
    {
        private readonly Dictionary<string, ClientConnection> _clients = new();
        private readonly Dictionary<uint, ClientConnection> _udpTokenToConnection = new();

        public void Add(ClientConnection client)
        {
            if(_clients.ContainsKey(client.UserId)|| _udpTokenToConnection.ContainsKey(client.UdpToken))
            {
                throw new ClientExistException();
            }

            _clients.Add(client.UserId, client);
            _udpTokenToConnection.Add(client.UdpToken, client);
        }

        public async Task Remove(string UserId)
        {
            var client = _clients[UserId];
            if (client == null) return;

            using (CancellationTokenSource tokenSource = new CancellationTokenSource()) {
                var token = tokenSource.Token;
                //await client.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Removed from session", token);
            }

            _clients.Remove(client.UserId);
            _udpTokenToConnection.Remove(client.UdpToken);
        }

        public bool TryGet(string UserId, out ClientConnection? client)
        {
            return _clients.TryGetValue(UserId, out client);
        }

        public bool tryGetByUdpToken(uint udpToken, out ClientConnection? client)
        {
            return _udpTokenToConnection.TryGetValue(udpToken, out client);
        }

        public IReadOnlyCollection<ClientConnection> GetAll()
        {
            return _clients.Values;
        }
    }
}
