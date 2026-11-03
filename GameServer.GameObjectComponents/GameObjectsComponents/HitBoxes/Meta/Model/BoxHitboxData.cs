using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model.Base;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model
{
    public struct BoxHitboxData
    {
        public Vector3 Center { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Size { get; set; }

    }
}
