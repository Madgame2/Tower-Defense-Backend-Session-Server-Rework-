using EntityLib.GameRoomExtentions;
using EntityLib.Models;
using EntityLib.Services;
using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.СomponentsForLightweightEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.Projectiles
{
    internal class ProjectileMoveSystem : ITickable
    {
        private Filter _projectilesFilter;

        private Stash<VelocityComponent> _velocityStash;
        private Stash<PossitionComponent> _positionStash;

        public void OnAwake(GameRoom world)
        {
            _projectilesFilter = Filter.Create()
                .With<ProjectileTag>()
                .With<VelocityComponent>()
                .With<PossitionComponent>()
                .Create();

            _velocityStash = world.GetStash<VelocityComponent>();
            _positionStash = world.GetStash<PossitionComponent>();
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            foreach(var projectileEntity in world.GetEnities(_projectilesFilter))
            {
                ref var positionComponent = ref _positionStash.Get(projectileEntity);
                ref var velocityComponent = ref _velocityStash.Get(projectileEntity);


                positionComponent.Position += velocityComponent.Velocity * delta;
            }
        }
    }
}
