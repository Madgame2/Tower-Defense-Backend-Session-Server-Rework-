using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Builder;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Extentions;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Security;
using System.Numerics;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.ResolveSystems
{
    internal class HorizontalResolveSystem : ITickable
    {
        private QueryExecutePlan<StaticTreeData> _treeQuery;
        private QueryContext<StaticTreeData> queryContext;

        private HitBoxComponent _hitBoxComponent;

        private List<StaticTreeData> buffer = new(32);

        private IHitBoxCollisionService _hitBoxCollisionService;



        private HitBoxComponent _palyerHitBox;

        public HorizontalResolveSystem(IHitBoxCollisionService hitBoxCollisionService)
        {
            _hitBoxCollisionService = hitBoxCollisionService;
        }

        public void OnAwake(GameRoom world)
        {
            _palyerHitBox = new HitBoxComponent();

            _palyerHitBox.Data.ShapeType = Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums.HitboxShapeType.Capsule;
            _palyerHitBox.Data.Shape.Capsule = new Application.Components.GameObjectsComponents.HitBoxes.Meta.Model.CapsuleHitboxData();
            _palyerHitBox.Data.Shape.Capsule.Radius = 0.5f;
            _palyerHitBox.Data.Shape.Capsule.Height = 2f;
            _palyerHitBox.Data.Shape.Capsule.Center = new Vector3(0,1,0);

            _treeQuery = world.worldQueryService
                .Search<StaticTreeData>()
                .InRadius("PlayerPos", 1f)
                .Build(out var query1Context);

            queryContext = query1Context;


            var obstacleGameObject = world.StaticResources.GetByName("Tree_1");
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
            var players = world.AllPlayers;

            foreach (var player in players)
            {
                if (!player.TryGetComponent<TransformComponent>(out var transformCompoent))
                    continue;

                Matrix4x4 playerTransform = Matrix4x4.CreateTranslation(transformCompoent.Position);

                queryContext.Set("PlayerPos", transformCompoent.Position);
                world.worldQueryService.ExecuteQuery(_treeQuery, queryContext, buffer);

                foreach (var entity in buffer)
                {
                    var treePosition = new Vector3(entity.LocalPosition.X, 0, entity.LocalPosition.Y);
                    var treeRotation = entity.Rotation;

                    Matrix4x4 treeTransform =
                        Matrix4x4.CreateScale(entity.Scale) *
                        Matrix4x4.CreateFromQuaternion(entity.Rotation) *
                        Matrix4x4.CreateTranslation(treePosition);

                    Matrix4x4 treeHitboxWorldMatrix = _hitBoxComponent.LocalMatrix * treeTransform;

                    if (_hitBoxCollisionService.TryComputePenetration(
                        in _palyerHitBox,
                        in playerTransform,
                        in _hitBoxComponent,
                        in treeHitboxWorldMatrix,
                        out Vector3 direction,
                        out float depth))
                    {
                        transformCompoent.Position -= direction * depth;
                    }
                }
            }
        }
    }
}
