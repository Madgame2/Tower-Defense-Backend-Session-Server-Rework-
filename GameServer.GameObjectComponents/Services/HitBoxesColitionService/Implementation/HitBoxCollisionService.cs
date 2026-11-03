using GameServer.Application.Components.GameObjectsComponents;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.ChcekModulesRegister.Interfaces;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.Implementation
{
    public class HitBoxCollisionService : IHitBoxCollisionService
    {
        private readonly ICollisionCheckStateryRegister _collisionCheckStateryRegister;


        public HitBoxCollisionService(ICollisionCheckStateryRegister collisionCheckStateryRegister)
        {
            _collisionCheckStateryRegister = collisionCheckStateryRegister;
        }

        public bool TryComputePenetration(
            in HitBoxComponent firstObject,
            in Matrix4x4 firstObjectTransform,
            in HitBoxComponent secondObject,
            in Matrix4x4 secondObjectTransform,
            out Vector3 direction,
            out float depth)
        {

            if(!_collisionCheckStateryRegister.TryGetCheckModule(firstObject.ShapeType, secondObject.ShapeType, out var checkModule))
            {
                direction = default;
                depth = 0f;

                throw new NotSupportedException(
                    $"No collision check module registered for " +
                    $"{firstObject.ShapeType} and {secondObject.ShapeType}.");

            }

            return checkModule.TryComputePenetration(
                    in firstObject,
                    in firstObjectTransform,
                    in secondObject,
                    in secondObjectTransform,
                    out direction,
                    out depth);
        }
    }
}
