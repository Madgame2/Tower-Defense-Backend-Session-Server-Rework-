using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class ApplyPlayerMoveInputSystem : ITickable
    {
        public async Task Tick(float delta, GameRoom world)
        {
            var players = world.AllPlayers;
            foreach (var player in players) { 

                var playerInput = player.InputBuffer.FetchNextInput();

                player.Velocity.X = playerInput.MoveDirection.X * player.Speed;
                player.Velocity.Z = playerInput.MoveDirection.Z * player.Speed;

                player.IsJumping = playerInput.JumpRequest;

                //player.Position += playerInput.MoveDirection* player.Speed * delta;
            }
        }
    }
}
