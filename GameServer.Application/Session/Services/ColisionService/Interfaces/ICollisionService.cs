using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Domain.ColliderSystem.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Session.Services.ColisionService.Interfaces
{
    public interface ICollisionService
    {
        bool TryComputePenetration(
            in ColliderShape playerColider,
            in Matrix4x4 playerTransform,
            in HitBoxComponent targetHitBox,
            in Matrix4x4 targetTransform,
            out Vector3 direction,
            out float depth
            );
    }
}
