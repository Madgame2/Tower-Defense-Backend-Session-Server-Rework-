using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class VelocityToPositionSystem : ITickable
    {
        public async Task Tick(float delta, GameRoom world)
        {
            var allPlayers = world.AllPlayers;

            foreach (var player in allPlayers)
            {
                player.Position += player.Velocity * delta;
            }
        }
    }
}
