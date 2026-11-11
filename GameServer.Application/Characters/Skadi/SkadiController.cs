using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.Characters.model;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload.Player;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterController;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Phisycs;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Characters.Skadi
{
    public class SkadiController : CharacterController
    {
        private const float TEMP_SPEED = 2f;
        private const float TEMP_JUMP_FORCE = 5f;

        public override void ProceeLook(Vector3 lookPoint)
        {
            var myPlayer = (Player)GameObject;

            myPlayer.LookAt = lookPoint;
        }

        public override void ProcessAlternativeAction(PlayerActionState actionState, float delta)
        {
            if (actionState.WasPressed)
            {
                StartAiming();
            }

            if (actionState.WasReleased)
            {
                StopAiming();
            }
        }

        public override void ProcessJump()
        {
            var myPlayer = (Player)GameObject;

            if (myPlayer.TryGetComponent<VelocityComponent>(out var component))
            {
                if (myPlayer.IsGrounded)
                {
                    myPlayer.MovementState = Domain.Player.Enums.MovementState.Jumping;
                    component.Velocity.Y = TEMP_JUMP_FORCE;

                    myPlayer.IsGrounded = false;
                    myPlayer.IsJumping = false;
                }
            }
        }

        public override void ProcessMove(Vector3 moveDirection)
        {
            var myPlayer = GameObject;

            if(myPlayer.TryGetComponent<VelocityComponent>(out var component))
            {
                component.Velocity.X = moveDirection.X * TEMP_SPEED;
                component.Velocity.Z = moveDirection.Z * TEMP_SPEED;
            }
        }

        public override void ProcessRotation(Quaternion inputRotation)
        {
            var myPlayer = GameObject;

            if(myPlayer.TryGetComponent<TransformComponent>(out var component))
            {
                component.Rotation = inputRotation;
            }
        }

        private void StartAiming()
        {
            var myPlayer = (Player)GameObject;

            var playereventpayload = new PlayerGameplayEventPayload
            {
                EventType = PlayerGameplayEventType.ContinuousActionStarted,
                ActionId = (ushort)Skadi.Ability.AbilityType.Aiming,
            };

            var payload = new GameplayeEventPayload
            {
                PlayerEvent = playereventpayload
            };

            _onReplication?.Invoke(ServerEvent.GamePlay(myPlayer.ObjectId, ObjectType.Player, payload));
        }

        private void StopAiming()
        {
            var myPlayer = (Player)GameObject;

            var playereventpayload = new PlayerGameplayEventPayload
            {
                EventType = PlayerGameplayEventType.ContinuousActionCancelled,
                ActionId = (ushort)Skadi.Ability.AbilityType.Aiming,
            };

            var payload = new GameplayeEventPayload
            {
                PlayerEvent = playereventpayload
            };

            _onReplication?.Invoke(ServerEvent.GamePlay(myPlayer.ObjectId, ObjectType.Player, payload));
        }
    }
}
