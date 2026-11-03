using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model.Union
{
    [StructLayout(LayoutKind.Explicit)]
    public struct HitboxShapeUnion
    {
        [FieldOffset(0)]
        public SphereHitboxData Sphere;

        [FieldOffset(0)]
        public CapsuleHitboxData Capsule;

        [FieldOffset(0)]
        public BoxHitboxData Box;
    }
}
