using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model.Base
{
    public struct HitboxDataBase
    {
        public Vector3 Center {  get; set; }
        public Quaternion Rotation {  get; set; }
    }
}
