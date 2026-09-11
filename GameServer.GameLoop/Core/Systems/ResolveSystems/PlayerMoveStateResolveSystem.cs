using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.ResolveSystems
{
    internal class PlayerMoveStateResolveSystem : ITickable
    {
        public async Task Tick(float delta, GameRoom world)
        {
            var players = world.AllPlayers;

            foreach (var player in players)
            {
                if(!player.IsGrounded)
                {
                    player.MovementState = Domain.Player.Enums.MovementState.Falling;
                }
            }
        }
    }
}
