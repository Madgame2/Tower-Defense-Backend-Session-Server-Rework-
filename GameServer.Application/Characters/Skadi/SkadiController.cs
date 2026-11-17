using EntityLib.GameRoomExtentions;
using EntityLib.Models;
using GameServer.Application.Services.LightEnityesSpawner;
using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.Characters.model;
using GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload.Player;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterController;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.SpawnPointAnchor;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Phisycs;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using GameServer.СomponentsForLightweightEntities;
using GameServer.Application.Services.LightEnityesSpawnerService.Extentions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using VelocityComponent = GameServer.СomponentsForLightweightEntities.VelocityComponent;
using GameServer.Domain.SessionWorld.Interfaces;

namespace GameServer.Application.Characters.Skadi
{
    public class SkadiController : CharacterController
    {
        private const float TEMP_SPEED = 2f;
        private const float TEMP_JUMP_FORCE = 5f;
        private const float TEMP_ARROW_FORECE = 20f;
        private bool _isAiming;

        private readonly ILightEntitySpawner<Entity> _lightEnitySpawner;
        private readonly ISessionStaticResources _staticResources;

        public SkadiController(ILightEntitySpawner<Entity> lightSpawner, ISessionStaticResources staticResources)
        {
            _lightEnitySpawner = lightSpawner;
            _staticResources = staticResources;
        }

        public override void ProceeLook(Vector3 lookPoint)
        {
            var myPlayer = (Player)GameObject;

            myPlayer.LookAt = lookPoint;
        }

        public override void ProcessAlternativeAction(PlayerActionState actionState, float delta)
        {
            if (actionState.WasPressed && !_isAiming)
            {
                StartAiming();
                _isAiming = true;
            }

            if (actionState.WasReleased && _isAiming)
            {
                StopAiming();
                _isAiming = false;
            }
        }

        public override void ProcessJump()
        {
            var myPlayer = (Player)GameObject;

            if (myPlayer.TryGetComponent<GameObjectComponentsAndServices.GameObjectsComponents.Phisycs.VelocityComponent>(out var component))
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

            if(myPlayer.TryGetComponent<GameObjectComponentsAndServices.GameObjectsComponents.Phisycs.VelocityComponent>(out var component))
            {
                component.Velocity.X = moveDirection.X * TEMP_SPEED;
                component.Velocity.Z = moveDirection.Z * TEMP_SPEED;
            }
        }

        public override void ProcessPrimaryAction(PlayerActionState actionState, float delta)
        {
            if (actionState.WasPressed)
                ShootArrow();
                
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

        private void ShootArrow()
        {
            var player = (Player)GameObject;
            var gameRoom = player.LinkedRoom;

            if (!player.TryGetComponent<SpawnPointAnchorComponent>(out var component))
                return;

            var fromspawPoitnToLookAt = player.LookAt - component.WorldSpawnPoint ;
            var direction = Vector3.Normalize(fromspawPoitnToLookAt);


            Quaternion rotation = DirectionToQuaternion(direction,Vector3.UnitY);
            var prefab = _staticResources.GetByName("Arrow_deffoult");

            var entity = _lightEnitySpawner.CreateEntity()
                .With(new ProjectileTag())
                .With(new PhysicalObjectTag())
                .With(new PrefabComponent { Prefab = prefab })
                .With(new PossitionComponent { Position = component.WorldSpawnPoint, })
                .With(new RotationComponent { Rotation = rotation })
                .With(new VelocityComponent {Velocity = direction* TEMP_ARROW_FORECE })
                .Build();

            var idComponent = gameRoom.GetStash<IDComponent>().Get(entity);
            var positionComponent = gameRoom.GetStash<PossitionComponent>().Get(entity);
            var rotationComponent = gameRoom.GetStash<RotationComponent>().Get(entity);

            var projjectilePayload = new ProjectilePayload
            {
                prefabId = 1,
                Position = positionComponent.Position,
                Rotation = rotationComponent.Rotation,
            };

            var payload = new SpawnPayload
            {
                projectilePayload = projjectilePayload
            };

            _onReplication?.Invoke(ServerEvent.Created(idComponent.ObjectId, ObjectType.Projectile, payload));
        }

        private static Quaternion DirectionToQuaternion(Vector3 direction, Vector3 upVector)
        {
            // System.Numerics: forward = -Z, Unity: forward = +Z, поэтому инвертируем
            var matrix = Matrix4x4.CreateWorld(Vector3.Zero, -direction, upVector);
            return Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(matrix));
        }
    }
}
