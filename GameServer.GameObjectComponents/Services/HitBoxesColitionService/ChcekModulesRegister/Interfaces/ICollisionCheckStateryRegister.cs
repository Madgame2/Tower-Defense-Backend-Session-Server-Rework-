using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.ChcekModulesRegister.Interfaces
{
    public interface ICollisionCheckStateryRegister
    {
        bool TryGetCheckModule(HitboxShapeType shape1,  HitboxShapeType shape2, out CollisionCheckModule outModule);
    }
}
