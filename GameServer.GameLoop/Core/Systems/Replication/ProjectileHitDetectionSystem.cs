using EntityLib.GameRoomExtentions;
using EntityLib.Models;
using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Extentions;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.Interfaces;
using GameServer.СomponentsForLightweightEntities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.Replication
{
    internal class ProjectileHitDetectionSystem : ITickable
    {
        private QueryExecutePlan<StaticTreeData> _staticDataQuery;
        private QueryContext<StaticTreeData> _staticDataQueryContext;
        private List<StaticTreeData> _staticObjectsBuffer = new(16);


        private QueryExecutePlan<HitBoxComponent> _dynamicObjectsQuery;
        private QueryContext<HitBoxComponent> _dynmicObjectsQueryContext;
        private List<HitBoxComponent> _dynamicObjectsBuffer = new(16);


        private readonly ISessionStaticResources _staticResources;
        private readonly IHitBoxCollisionService _hitboxCollisionService;
        private readonly ILogger<ProjectileHitDetectionSystem> _logger;


        private HitBoxComponent _hitBoxComponent;


        private Filter _projectileFilter;
        private Stash<PossitionComponent> _possitionStash;
        private Stash<RotationComponent> _rotationStash;
        private Stash<PhysicalObjectTag> _physicalStash;
        private Stash<VelocityComponent> _velocityStash;


        public ProjectileHitDetectionSystem(
            IHitBoxCollisionService collisonService,
            ISessionStaticResources sessionStaticResources,
            ILogger<ProjectileHitDetectionSystem> logger)
        {
            _hitboxCollisionService = collisonService;
            _staticResources = sessionStaticResources;
            _logger = logger;
        }


        public void OnAwake(GameRoom world)
        {
            _projectileFilter = Filter.Create()
                .With<PhysicalObjectTag>()
                .With<ProjectileTag>()
                .With<PossitionComponent>()
                .With<RotationComponent>()
                .Create();

            _possitionStash = world.GetStash<PossitionComponent>();
            _rotationStash = world.GetStash<RotationComponent>();
            _physicalStash = world.GetStash<PhysicalObjectTag>();
            _velocityStash = world.GetStash<VelocityComponent>();

            _staticDataQuery = world.worldQueryService
                .Search<StaticTreeData>()
                .InRadius("projectilePosition", 1f)
                .Build(out _staticDataQueryContext);

            _dynamicObjectsQuery = world.worldQueryService
                .Search<HitBoxComponent>()
                .InRadius("projectilePosition", 1f)
                .Build(out _dynmicObjectsQueryContext);

            var obstacleGameObject = _staticResources.GetByName("Tree_1");
            if (obstacleGameObject != null)
            {
                bool hasCompoent = obstacleGameObject.TryGetComponents<HitBoxComponent>(out List<HitBoxComponent> outCompoents);
                if (hasCompoent)
                {
                    _hitBoxComponent = outCompoents[0];
                }
            }
        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            foreach(var projectile in world.GetEnities(_projectileFilter))
            {
                ref var possitionComponent = ref _possitionStash.Get(projectile);
                ref var rotationComponent = ref _rotationStash.Get(projectile);


                Matrix4x4 projectileTransform =
                    Matrix4x4.CreateScale(Vector3.One) *
                    Matrix4x4.CreateFromQuaternion(rotationComponent.Rotation) *
                    Matrix4x4.CreateTranslation(possitionComponent.Position);

                _staticDataQueryContext.Set("projectilePosition", possitionComponent.Position);
                world.worldQueryService.ExecuteQuery(_staticDataQuery, _staticDataQueryContext, _staticObjectsBuffer);

                foreach (var staticCandidate in _staticObjectsBuffer)
                {
                    var treePosition = new Vector3(staticCandidate.LocalPosition.X, 0, staticCandidate.LocalPosition.Y);
                    var treeRotation = staticCandidate.Rotation;

                    Matrix4x4 treeTransform =
                        Matrix4x4.CreateScale(staticCandidate.Scale) *
                        Matrix4x4.CreateFromQuaternion(staticCandidate.Rotation) *
                        Matrix4x4.CreateTranslation(treePosition);

                    Matrix4x4 treeHitboxWorldMatrix = _hitBoxComponent.LocalMatrix * treeTransform;

                    if (_hitboxCollisionService.TryComputePenetration(
                        in _hitBoxComponent,
                        in projectileTransform,
                        in _hitBoxComponent,
                        in treeHitboxWorldMatrix,
                        out Vector3 direction,
                        out float depth))
                    {
                        _logger.LogInformation("ПОПАЛ");
                        _physicalStash.Remove(projectile);
                        _velocityStash.Remove(projectile);
                    }
                }


                _dynmicObjectsQueryContext.Set("projectilePosition", possitionComponent.Position);
                world.worldQueryService.ExecuteQuery(_dynamicObjectsQuery, _dynmicObjectsQueryContext, _dynamicObjectsBuffer);
            }
        }
    }
}
