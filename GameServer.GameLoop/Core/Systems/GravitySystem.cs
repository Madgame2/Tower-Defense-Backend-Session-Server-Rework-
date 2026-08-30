using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class GravitySystem : ITickable
    {
        private const float Gravity = 9.8f;
        private const float TerminalVelocity = -50f;

        public async Task Tick(float delta, GameRoom world)
        {
            var allPlayers = world.AllPlayers;

            foreach (var player in allPlayers)
            {
                if (player.IsGrounded)
                    continue;

                player.Velocity.Y -= Gravity * delta;

                if(player.Velocity.Y < TerminalVelocity)
                {
                    player.Velocity.Y = TerminalVelocity;
                }
            }
        }
    }
}
