using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules.Base;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules
{
    public class CapsuleVsCapsule : ShapesCompareModule
    {
        public override HitboxShapeType ShapeType1 =>
            HitboxShapeType.Capsule;

        public override HitboxShapeType ShapeType2 =>
            HitboxShapeType.Capsule;

        public override bool TryComputePenetration(
            in HitBoxComponent firstObject,
            in Matrix4x4 firstObjectTransform,
            in HitBoxComponent secondObject,
            in Matrix4x4 secondObjectTransform,
            out Vector3 direction,
            out float depth)
        {
            var first = firstObject.Data.Shape.Capsule;
            var second = secondObject.Data.Shape.Capsule;

            GetWorldCapsule(
                first,
                firstObjectTransform,
                out var firstA,
                out var firstB,
                out var firstRadius);

            GetWorldCapsule(
                second,
                secondObjectTransform,
                out var secondA,
                out var secondB,
                out var secondRadius);

            // Находим ближайшие точки на осях капсул.
            ClosestPointsOnSegments(
                firstA,
                firstB,
                secondA,
                secondB,
                out var pointA,
                out var pointB);

            var delta = pointB - pointA;
            var distanceSquared = delta.LengthSquared();
            var radiusSum = firstRadius + secondRadius;

            // Нет пересечения или касание без проникновения.
            if (distanceSquared >= radiusSum * radiusSum)
            {
                direction = Vector3.Zero;
                depth = 0f;
                return false;
            }

            var distance = MathF.Sqrt(distanceSquared);

            // Направление от первой капсулы ко второй.
            if (distance > 1e-6f)
            {
                direction = delta / distance;
            }
            else
            {
                // Оси пересекаются: используем разницу центров.
                var firstCenter = (firstA + firstB) * 0.5f;
                var secondCenter = (secondA + secondB) * 0.5f;
                var centerDelta = secondCenter - firstCenter;

                direction = centerDelta.LengthSquared() > 1e-12f
                    ? Vector3.Normalize(centerDelta)
                    : Vector3.UnitX;
            }

            depth = radiusSum - distance;
            return true;
        }

        private static void GetWorldCapsule(
            CapsuleHitboxData capsule,
            in Matrix4x4 objectTransform,
            out Vector3 pointA,
            out Vector3 pointB,
            out float worldRadius)
        {
            // Предполагаем, что ось капсулы направлена вдоль локальной Y.
            // Height включает полусферические окончания.
            var halfSegment = MathF.Max(
                0f,
                capsule.Height * 0.5f - capsule.Radius);

            var localAxis = Vector3.Transform(
                Vector3.UnitY,
                capsule.Rotation);

            var localA = capsule.Center - localAxis * halfSegment;
            var localB = capsule.Center + localAxis * halfSegment;

            pointA = Vector3.Transform(localA, objectTransform);
            pointB = Vector3.Transform(localB, objectTransform);

            // Для неравномерного масштаба используем максимальный масштаб
            // по осям объекта — это консервативное приближение радиуса.
            var scaleX = new Vector3(
                objectTransform.M11,
                objectTransform.M12,
                objectTransform.M13).Length();

            var scaleY = new Vector3(
                objectTransform.M21,
                objectTransform.M22,
                objectTransform.M23).Length();

            var scaleZ = new Vector3(
                objectTransform.M31,
                objectTransform.M32,
                objectTransform.M33).Length();

            var maxScale = MathF.Max(scaleX, MathF.Max(scaleY, scaleZ));
            worldRadius = capsule.Radius * maxScale;
        }

        private static void ClosestPointsOnSegments(
            Vector3 p1,
            Vector3 q1,
            Vector3 p2,
            Vector3 q2,
            out Vector3 c1,
            out Vector3 c2)
        {
            const float epsilon = 1e-8f;

            var d1 = q1 - p1;
            var d2 = q2 - p2;
            var r = p1 - p2;

            var a = Vector3.Dot(d1, d1);
            var e = Vector3.Dot(d2, d2);
            var f = Vector3.Dot(d2, r);

            float s;
            float t;

            if (a <= epsilon && e <= epsilon)
            {
                // Обе оси выродились в точки.
                c1 = p1;
                c2 = p2;
                return;
            }

            if (a <= epsilon)
            {
                // Первая ось — точка.
                s = 0f;
                t = Math.Clamp(f / e, 0f, 1f);
            }
            else
            {
                var c = Vector3.Dot(d1, r);

                if (e <= epsilon)
                {
                    // Вторая ось — точка.
                    t = 0f;
                    s = Math.Clamp(-c / a, 0f, 1f);
                }
                else
                {
                    var b = Vector3.Dot(d1, d2);
                    var denominator = a * e - b * b;

                    s = denominator > epsilon
                        ? Math.Clamp((b * f - c * e) / denominator, 0f, 1f)
                        : 0f;

                    t = (b * s + f) / e;

                    if (t < 0f)
                    {
                        t = 0f;
                        s = Math.Clamp(-c / a, 0f, 1f);
                    }
                    else if (t > 1f)
                    {
                        t = 1f;
                        s = Math.Clamp((b - c) / a, 0f, 1f);
                    }
                }
            }

            c1 = p1 + d1 * s;
            c2 = p2 + d2 * t;
        }
    }
}
