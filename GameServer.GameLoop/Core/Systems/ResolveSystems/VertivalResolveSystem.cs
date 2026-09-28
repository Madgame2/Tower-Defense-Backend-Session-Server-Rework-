using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.ResolveSystems
{
    internal class VertivalResolveSystem : ITickable
    {
        public async Task Tick(float delta, GameRoom world)
        {
            var allPlayers = world.AllPlayers;

            foreach (var player in allPlayers)
            {
                var playerPosition = player.Position;

                var terrainY = await world.worldQueryService.GetHeightAt(world,playerPosition.X, playerPosition.Z);

                float pivotOffsetY = -player.Pivot.Y * player.Size.Y / 2f;

                float bottomOffset =pivotOffsetY + player.Colider.GetBottomOffset();


                float bottomY = playerPosition.Y + bottomOffset;

                if (bottomY <= terrainY)
                {
                    float correction =
                        terrainY - bottomY;

                    playerPosition.Y += correction;

                    player.Velocity.Y = 0;
                    player.MovementState = Domain.Player.Enums.MovementState.Grounded;
                    player.Position = playerPosition;
                    player.IsGrounded = true;
                }
                else
                {
                    player.IsGrounded = false;

                }
            }
        }
    }
}
