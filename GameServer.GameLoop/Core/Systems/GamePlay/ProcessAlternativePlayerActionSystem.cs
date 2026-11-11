using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Characters.model;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterController;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.GamePlay
{
    internal class ProcessAlternativePlayerActionSystem : ITickable
    {
        private readonly Dictionary<uint, bool> _previousStates = new();

        public void OnAwake(GameRoom world)
        {

        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var allPlayers = world.AllPlayers;
            foreach (var player in allPlayers)
            {
                if (!player.TryGetComponent<CharacterController>(out var controller))
                {
                    throw new ArgumentException($"Not found controller at player object: {player}");
                }

                bool isPressed = player.CurrentStateSnapshot.AlternativeActonFlag;

                _previousStates.TryGetValue(player.PlayerId, out bool wasPressed);

                var actionState = new PlayerActionState
                {
                    IsPressed = isPressed,
                    WasPressed = isPressed && !wasPressed,
                    WasReleased = !isPressed && wasPressed
                };

                _previousStates[player.ObjectId] = isPressed;

                if (alternativeActon)
                    controller.ProcessAlternativeAction();
            }
        }
    }
}
