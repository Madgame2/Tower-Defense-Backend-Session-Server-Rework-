using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterController;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.GamePlay
{
    internal class MovementSystem : ITickable
    {
        public void OnAwake(GameRoom world)
        {
            
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var allPlayers = world.AllPlayers;
            foreach (var player in allPlayers)
            {
                if(!player.TryGetComponent<CharacterController>(out var controller))
                {
                    throw new ArgumentException($"Not found controller at player object: {player}");
                }

                var moveDirection = player.CurrentStateSnapshot.MoveDirection;

                controller.ProcessMove(moveDirection);
            }
        }
    }
}
