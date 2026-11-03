using GameServer.Application.Components.GameObjectsComponents;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.Interfaces
{
    public interface IHitBoxCollisionService
    {
        bool TryComputePenetration(
            in HitBoxComponent firestObject,
            in Matrix4x4 firstObjectTransform,
            in HitBoxComponent secondObject,
            in Matrix4x4 secondObjectTransform,
            out Vector3 direction,
            out float depth);
    }
}
