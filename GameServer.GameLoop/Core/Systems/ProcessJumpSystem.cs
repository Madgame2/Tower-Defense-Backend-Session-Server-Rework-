using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class ProcessJumpSystem : ITickable
    {
        public void OnAwake(GameRoom world)
        {

        }

        public async Task Tick(float delta, GameRoom world)
        {
            var players = world.AllPlayers;

            foreach (var player in players)
            {
                if(player.IsGrounded&& player.IsJumping)
                {
                    player.MovementState = Domain.Player.Enums.MovementState.Jumping;
                    player.Velocity.Y = 5;

                    player.IsGrounded = false;
                    player.IsJumping = false;
                }
            }
        }
    }
}
