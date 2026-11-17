using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.Replication
{
    internal class DeleteOldEventsSystem : ITickable
    {
        private ReplicationLog _log;


        public void OnAwake(GameRoom world)
        {
            _log = world.Logs;
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var allPlayers = world.AllPlayers;

            uint oldestProcessedTick = 0;
            foreach (var player in allPlayers)
            {
                if (oldestProcessedTick == 0)
                    oldestProcessedTick = player.LastAcknowledgedTick;

                oldestProcessedTick = oldestProcessedTick > player.LastAcknowledgedTick ? player.LastAcknowledgedTick : oldestProcessedTick;
            }

            _log.RemoveThrough(oldestProcessedTick);
        }
    }
}
