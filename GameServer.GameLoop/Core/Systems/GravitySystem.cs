using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Phisycs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class GravitySystem : ITickable
    {
        private const float Gravity = 9.8f;
        private const float TerminalVelocity = -50f;

        public void OnAwake(GameRoom world)
        {

        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var allPlayers = world.AllPlayers;

            foreach (var player in allPlayers)
            {
                if (player.IsGrounded)
                    continue;

                if (player.TryGetComponent<VelocityComponent>(out var component))
                {

                    component.Velocity.Y -= Gravity * delta;

                    if (component.Velocity.Y < TerminalVelocity)
                    {
                        component.Velocity.Y = TerminalVelocity;
                    }
                }
            }
        }
    }
}
