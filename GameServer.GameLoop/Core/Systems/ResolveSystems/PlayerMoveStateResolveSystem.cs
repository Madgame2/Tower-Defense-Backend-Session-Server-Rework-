using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Phisycs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.ResolveSystems
{
    internal class PlayerMoveStateResolveSystem : ITickable
    {
        public void OnAwake(GameRoom world)
        {
            
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var players = world.AllPlayers;

            foreach (var player in players)
            {
                if (player.TryGetComponent<VelocityComponent>(out var component))
                {
                    if (!player.IsGrounded && component.Velocity.Y <= -2)
                    {
                        player.MovementState = Domain.Player.Enums.MovementState.Falling;
                    }
                }
            }
        }
    }
}
