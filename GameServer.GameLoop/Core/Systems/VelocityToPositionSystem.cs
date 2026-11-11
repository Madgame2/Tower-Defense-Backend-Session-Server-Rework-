using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Phisycs;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class VelocityToPositionSystem : ITickable
    {

        public void OnAwake(GameRoom world)
        {

        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var allPlayers = world.AllPlayers;

            foreach (var player in allPlayers)
            {
                if (!player.TryGetComponent<TransformComponent>(out var transformComponent))
                    continue;

                if (!player.TryGetComponent<VelocityComponent>(out var velocityComponent))
                    continue;

                transformComponent.Position += velocityComponent.Velocity * delta;
            }
        }
    }
}
