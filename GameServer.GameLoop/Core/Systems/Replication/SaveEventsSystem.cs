using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.Replication
{
    public class SaveEventsSystem : ITickable
    {
        private IReplicationRegistry _replicationRegistry;
        private ReplicationLog _log;


        private readonly List<ServerEvent> _events = new();
        public void OnAwake(GameRoom world)
        {
            _replicationRegistry = world.ReplicationRegistry;
            _log = world.Logs;
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            _replicationRegistry.CollectChanges(_events);

            foreach (ServerEvent e in _events)
            {
                _log.AddEntry(tick, e);
            }
        }
    }
}
