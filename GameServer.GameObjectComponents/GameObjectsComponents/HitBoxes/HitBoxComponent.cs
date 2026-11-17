using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model.Union;
using GameServer.Domain.Core.GameObjects.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Components.GameObjectsComponents
{
    public class HitBoxComponent: ComponentBase
    {
        public HitboxData Data;
        public HitboxShapeType ShapeType => Data.ShapeType;

        public Matrix4x4 LocalMatrix => Data.ShapeType switch
        {
            HitboxShapeType.Box =>
                GetLocalMatrix(Data.Shape.Box.Center,
                               Data.Shape.Box.Rotation),

            HitboxShapeType.Capsule =>
                GetLocalMatrix(Data.Shape.Capsule.Center,
                               Data.Shape.Capsule.Rotation),

            HitboxShapeType.Sphere =>
                GetLocalMatrix(Data.Shape.Sphere.Center,
                               Quaternion.Identity),

            _ => throw new NotSupportedException(
                $"Unsupported hitbox type: {Data.ShapeType}")
        };


        private static Matrix4x4 GetLocalMatrix(Vector3 center, Quaternion rotation)
        {
            return Matrix4x4.CreateFromQuaternion(rotation)
                 * Matrix4x4.CreateTranslation(center);
        }

        public override ComponentBase Clone()
        {
            return new HitBoxComponent
            {
                Data = new HitboxData
                {
                    ShapeType = Data.ShapeType,
                    Shape = CloneShape(Data.Shape, Data.ShapeType)
                }
            };
        }


        private static HitboxShapeUnion CloneShape(HitboxShapeUnion source, HitboxShapeType shapeType)
        {
            return shapeType switch
            {
                HitboxShapeType.Box => new HitboxShapeUnion
                {
                    Box = source.Box
                },

                HitboxShapeType.Capsule => new HitboxShapeUnion
                {
                    Capsule = source.Capsule
                },

                HitboxShapeType.Sphere => new HitboxShapeUnion
                {
                    Sphere = source.Sphere
                },

                _ => throw new NotSupportedException(
                    $"Unsupported hitbox type: {shapeType}")
            };
        }
    }
}
