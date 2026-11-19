using EntityLib.GameRoomExtentions;
using EntityLib.Models;
using GameServer.Application.Interfaces;
using GameServer.Contracts.UDP.Pakets;
using GameServer.Contracts.WorldStates.EntityState;
using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.UDP.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.СomponentsForLightweightEntities;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.NetworkSystems
{
    internal class EntitystateBroadcastSystem : INetworkTickable
    {
        private readonly IUdpSender _udpSender;
        private readonly IConnectedClientsStorage _connectedClinetStorage;


        private List<EntityState> _statesBuffer = new();

        private Filter _projectilesFilter;
        private Stash<IDComponent> _idStash;
        private Stash<PossitionComponent> _positionStash;
        private Stash<RotationComponent> _rotationStash;

        public EntitystateBroadcastSystem(IUdpSender udpSender, IConnectedClientsStorage connectedClientsStorage)
        {
            _udpSender = udpSender;
            _connectedClinetStorage = connectedClientsStorage;
        }

        public void OnAwake(GameRoom world)
        {
            _projectilesFilter = Filter.Create()
                .With<ProjectileTag>()
                .With<IDComponent>()
                .With<PossitionComponent>()
                .With<RotationComponent>()
                .Create();

            _idStash = world.GetStash<IDComponent>();
            _positionStash = world.GetStash<PossitionComponent>();
            _rotationStash = world.GetStash<RotationComponent>();
        }


        public void NetworkTick(float delta, uint tick, GameRoom world)
        {
            var stopwatch = Stopwatch.StartNew();

            _statesBuffer.Clear();

            foreach (var entity in world.GetEnities(_projectilesFilter))
            {
                ref var idComponent = ref _idStash.Get(entity);
                ref var positionComponent = ref _positionStash.Get(entity);
                ref var rotationComponent = ref _rotationStash.Get(entity);

                _statesBuffer.Add(
                    EntityState.ProjectileSnapshot(
                        idComponent.ObjectId,
                        positionComponent.Position,
                        rotationComponent.Rotation));
            }

           long collectTime = stopwatch.ElapsedTicks;

            ReadOnlySpan<EntityState> entries =
                CollectionsMarshal.AsSpan(_statesBuffer);

            int requiredSize = EntitesStatSnapshotWriter.GetSize(entries);

            byte[] buffer = ArrayPool<byte>.Shared.Rent(requiredSize);

            int written;

            try
            {
                written = EntitesStatSnapshotWriter.WriteSnapshot(
                    entries,
                    tick,
                    buffer.AsSpan(0, requiredSize));

                long serializeTime = stopwatch.ElapsedTicks - collectTime;

                SendSnapshot(world, buffer.AsMemory(0, written));

                stopwatch.Stop();

                ////Console.WriteLine(
                //    $"[NetworkTick] Entities: {entries.Length} | " +
                //    $"Collect: {collectTime * 1000.0 / Stopwatch.Frequency:F3} ms | " +
                //    $"Serialize: {serializeTime * 1000.0 / Stopwatch.Frequency:F3} ms | " +
                //    $"Total: {stopwatch.Elapsed.TotalMilliseconds:F3} ms | " +
                //    $"Bytes: {written}");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private void SendSnapshot(GameRoom gameRoom, Memory<byte> memory)
        {
            var players = gameRoom.AllPlayers;

            foreach (var player in players)
            {
                if (!_connectedClinetStorage.TryGet(player.UserId, out var clientConnection))
                    continue;

                if (clientConnection.ConnectionEndPoint == null) continue;

                _udpSender.SendAsync(clientConnection.ConnectionEndPoint,memory);
            }
        }
    }
}
