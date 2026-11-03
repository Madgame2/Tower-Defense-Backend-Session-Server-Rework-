using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules.Base
{
    public abstract class ShapesCompareModule : CollisionCheckModule
    {
        public abstract HitboxShapeType ShapeType1 { get; }
        public abstract HitboxShapeType ShapeType2 { get; }

    }
}
