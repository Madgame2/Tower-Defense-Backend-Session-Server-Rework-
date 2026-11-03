using GameServer.Application.Components.GameObjectsComponents;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules.Base
{
    public abstract class CollisionCheckModule
    {
        public abstract bool TryComputePenetration(in HitBoxComponent firstObject, in Matrix4x4 firstObjectTransform, in HitBoxComponent secondObject, in Matrix4x4 secondObjectTransform, out Vector3 direction, out float depth);
    }
}
