using EntityLib.GameRoomExtentions;
using EntityLib.Models;
using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.СomponentsForLightweightEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.Physical
{
    internal class LightEntityGravitySystem : ITickable
    {
        private Filter _physicalObjectsFilter;

        private const float TEMP_GRAVITY_FORCE = 9.8f;
        private const float TerminalVelocity = -50f;


        private Stash<VelocityComponent> _velocityStash;

        public void OnAwake(GameRoom world)
        {
            _physicalObjectsFilter = Filter.Create()
                .With<PhysicalObjectTag>()
                .With<VelocityComponent>()
                .Create();

            _velocityStash = world.GetStash<VelocityComponent>();
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            foreach (var entity in world.GetEnities(_physicalObjectsFilter))
            {
                ref var component = ref _velocityStash.Get(entity);

                component.Velocity.Y -= TEMP_GRAVITY_FORCE * delta;

                if (component.Velocity.Y < TerminalVelocity)
                {
                    component.Velocity.Y = TerminalVelocity;
                }

            }
        }
    }
}
