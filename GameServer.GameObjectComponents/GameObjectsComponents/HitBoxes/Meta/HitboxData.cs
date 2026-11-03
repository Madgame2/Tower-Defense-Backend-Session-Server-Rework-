using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model.Union;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta
{
    public struct HitboxData
    {
        public HitboxShapeType ShapeType;
        public HitboxShapeUnion Shape;
    }
}
