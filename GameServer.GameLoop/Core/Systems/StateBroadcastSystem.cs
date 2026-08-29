using GameServer.Application.Interfaces;
using GameServer.Contracts.UDP.Enums;
using GameServer.Contracts.UDP.Pakets;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.UDP.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class StateBroadcastSystem : INetworkTickable
    {
        private readonly IUdpSender _udpSender;
        private readonly IConnectedClientsStorage _connectedClinetStorage;

        public StateBroadcastSystem(IUdpSender udpSender, IConnectedClientsStorage connectedClientsStorage)
        {
            _udpSender = udpSender;
            _connectedClinetStorage = connectedClientsStorage;
        }

        public void NetworkTick(float delta, uint serverTick, GameRoom world)
        {
            var players = world.AllPlayers;

            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);

            foreach (var player in players)
            {
                if (!_connectedClinetStorage.TryGet(player.Id, out var clientConnection))
                    continue;

                if (clientConnection.ConnectionEndPoint == null) continue;

                var paket = new PlayerStateSnapshot(player.Id, serverTick, player.Position);

                try
                {
                    int length = paket.Serialize(buffer.AsSpan());

                    _ = _udpSender.SendAsync(clientConnection.ConnectionEndPoint, new ReadOnlyMemory<byte>(buffer, 0, length));

                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);

                }
            }
        }
    }
}
