using GameServer.Domain.Core.GameObjects.Components;
using GameServer.Domain.ValueObjects.Geometrics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform
{
    public class TransformComponent: ComponentBase
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public TransformComponent()
        {
            Position = Vector3.Zero;
            Rotation= Quaternion.Identity;
            Scale = Vector3.One;
        }

        public TransformSnapShoot GetShapShoot()
        {
            return new TransformSnapShoot
            {
                Position = Position,
                Rotation = Rotation,
                Scale = Scale,
            };
        }
    }
}
