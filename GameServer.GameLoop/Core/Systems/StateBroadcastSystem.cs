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

        public void OnAwake(GameRoom world)
        {

        }

        public void NetworkTick(float delta, uint serverTick, GameRoom world)
        {
            var players = world.AllPlayers;
            if (players.Length == 0) return;

            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                int length = PlayerStateSnapshotWriter.WriteSnapshot(buffer, serverTick, players);

                ReadOnlyMemory<byte> packetData = new ReadOnlyMemory<byte>(buffer, 0, length);

                foreach (var player in players)
                {
                    if (!_connectedClinetStorage.TryGet(player.Id, out var clientConnection))
                        continue;

                    if (clientConnection.ConnectionEndPoint == null) continue;

                    _udpSender.SendAsync(clientConnection.ConnectionEndPoint, packetData);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
