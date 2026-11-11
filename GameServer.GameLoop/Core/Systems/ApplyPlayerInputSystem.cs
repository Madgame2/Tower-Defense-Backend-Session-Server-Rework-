using GameServer.Domain.SessionWorld;
using GameServer.Domain.ValueObjects;
using GameServer.Domain.ValueObjects.Enums;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace GameServer.GameLoop.Core.Systems
{
    internal class ApplyPlayerInputSystem : ITickable
    {

        public void OnAwake(GameRoom world)
        {

        }
        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var players = world.AllPlayers;
            foreach (var player in players) { 

                var playerInput = player.InputBuffer.FetchNextInput();

                player.CurrentStateSnapshot = new CurrentClientActions(playerInput);

                //player.Velocity.X = playerInput.MoveDirection.X * player.Speed;
                //player.Velocity.Z = playerInput.MoveDirection.Z * player.Speed;

                //if(player.TryGetComponent<TransformComponent>(out var transform))
                //{
                //    transform.Rotation = playerInput.Rotation;
                //}

                //player.LookAt = playerInput.LookAtPoint;

                //player.IsJumping = (playerInput.Actions & ActionFlags.JumpRequested)!=0;
            }
        }
    }
}
