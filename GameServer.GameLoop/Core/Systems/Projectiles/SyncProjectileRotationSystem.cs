using EntityLib.GameRoomExtentions;
using EntityLib.Models;
using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.СomponentsForLightweightEntities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.Projectiles
{
    internal class SyncProjectileRotationSystem : ITickable
    {
        private Filter _projectilesFilter;

        private Stash<VelocityComponent> _velocityStash;
        private Stash<RotationComponent> _rotationStash;


        public void OnAwake(GameRoom world)
        {
            _projectilesFilter = Filter.Create()
                .With<ProjectileTag>()
                .With<VelocityComponent>()
                .With<RotationComponent>()
                .Create();

            _velocityStash = world.GetStash<VelocityComponent>();
            _rotationStash = world.GetStash<RotationComponent>();
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            foreach (var projectileEntity in world.GetEnities(_projectilesFilter))
            {
                ref var velocityComponent = ref _velocityStash.Get(projectileEntity);
                ref var rotationComponent = ref _rotationStash.Get(projectileEntity);


                var direction = Vector3.Normalize(velocityComponent.Velocity);



                rotationComponent.Rotation = LookRotation(direction, Vector3.UnitY);
            }
        }

        public static Quaternion LookRotation(Vector3 direction, Vector3 up)
        {
            direction = Vector3.Normalize(direction);

            // Если forward и up коллинеарны, выбираем запасной вектор вверх
            var matrix = Matrix4x4.CreateWorld(Vector3.Zero, -direction, up);
            return Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(matrix));
        }
    }
}
