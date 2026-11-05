using GameServer.Application.Interfaces;
using GameServer.Contracts.UDP.Pakets;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Preprocessor;
using GameServer.Domain.UDP.Interfaces;
using GameServer.Domain.UDP.Meta;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.Replication
{
    internal sealed class WorldReplicationBroadcastSystem : INetworkTickable
    {
        private ReplicationLog _log;

        private readonly IConnectedClientsStorage _connectedClientsStorage;
        private readonly ILogPreprocessor _logPreprocessor;
        private readonly IUdpSender _udpSender;

        public WorldReplicationBroadcastSystem(
            IConnectedClientsStorage connectedClientsStorage,
            ILogPreprocessor logPreprocessor,
            IUdpSender udpSender)
        {
            _connectedClientsStorage = connectedClientsStorage;
            _logPreprocessor = logPreprocessor;
            _udpSender = udpSender;
        }

        public void OnAwake(GameRoom world)
        {
            _log = world.Logs;
        }

        public void NetworkTick(
            float delta,
            uint tick,
            GameRoom world)
        {
            var allPlayers = world.AllPlayers;

            foreach (var player in allPlayers)
            {
                if (!_connectedClientsStorage.TryGet(
                        player.UserId,
                        out var connection))
                    continue;

                int itemsCount = _logPreprocessor.Process(
                    player.LastAcknowledgedTick,
                    _log,
                    out ReadOnlyMemory<ReplicationLogEntry> entriesToSend);

                if (itemsCount == 0)
                    continue;

                byte[] buffer = ArrayPool<byte>.Shared.Rent(1400);

                try
                {
                    int length = ReplicationLogWriter.WriteSnapshot(
                        entriesToSend,
                        tick,
                        buffer);

                    uint sentTick = tick;
                    uint playerId = player.PlayerId;

                    _ = _udpSender.SendReliableAsync(
                        connection.ConnectionEndPoint,
                        new ReadOnlyMemory<byte>(buffer, 0, length),
                        result =>
                        {
                            if (!result.IsAcknowledged)
                            {
                                return;
                            }
                            player.AcknowledgeTick(sentTick);
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
    }
}
