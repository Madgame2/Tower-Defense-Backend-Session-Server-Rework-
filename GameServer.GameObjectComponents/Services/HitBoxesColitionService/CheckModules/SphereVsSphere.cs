using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules.Base;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules
{
    internal class SphereVsSphere : ShapesCompareModule
    {
        public override HitboxShapeType ShapeType1 => HitboxShapeType.Sphere;

        public override HitboxShapeType ShapeType2 => HitboxShapeType.Sphere;

        public override bool TryComputePenetration(in HitBoxComponent firstObject, in Matrix4x4 firstObjectTransform, in HitBoxComponent secondObject, in Matrix4x4 secondObjectTransform, out Vector3 direction, out float depth)
        {
            var firstSphere = firstObject.Data.Shape.Sphere;
            var secondSphere = secondObject.Data.Shape.Sphere;

            Vector3 firstCenter = Vector3.Transform(
                firstSphere.Center, firstObjectTransform);

            Vector3 secondCenter = Vector3.Transform(
                secondSphere.Center, secondObjectTransform);

            Vector3 delta = secondCenter - firstCenter;

            float distanceSquared = delta.LengthSquared();
            float radiusSum = firstSphere.Radius + secondSphere.Radius;

            if (distanceSquared >= radiusSum * radiusSum)
            {
                direction = default;
                depth = 0f;
                return false;
            }

            if (distanceSquared > 0f)
            {
                float distance = MathF.Sqrt(distanceSquared);
                direction = delta / distance;
                depth = radiusSum - distance;
            }
            else
            {
                direction = Vector3.UnitX;
                depth = radiusSum;
            }

            return true;
        }
    }
}
