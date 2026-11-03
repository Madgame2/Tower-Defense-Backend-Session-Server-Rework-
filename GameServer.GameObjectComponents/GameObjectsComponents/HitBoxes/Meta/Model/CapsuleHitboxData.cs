using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model
{
    public struct CapsuleHitboxData
    {
        public Vector3 Center { get; set; }
        public Quaternion Rotation { get; set; }
        public float Radius {  get; set; }
        public float Height {  get; set; }
    }
}
