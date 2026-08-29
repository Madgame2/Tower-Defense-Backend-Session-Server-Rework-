using GameServer.Domain.ColliderSystem.Enum;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.ColliderSystem.Core
{
    public struct ColliderShape
    {
        public ColliderType Type;
        public Vector3 Offset;
        public Vector3 Size;
        public float Radius;
        public float Height;


        public float GetBottomOffset()
        {
            return Type switch
            {
                ColliderType.Sphere =>
                    Offset.Y - Radius,

                ColliderType.Box =>
                    Offset.Y - Size.Y / 2f,

                ColliderType.Capsule =>
                    Offset.Y - Height / 2f,

                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
