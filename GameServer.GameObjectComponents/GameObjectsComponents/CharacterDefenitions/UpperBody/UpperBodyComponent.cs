using GameServer.Domain.Core.GameObjects.Components;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.UpperBody
{
    public class UpperBodyComponent : ComponentBase
    {
        // Запечённая геометрия относительно PlayerRoot.
        public Vector3 PivotLocalPosition { get; }

        public Vector3 AimOriginOffsetFromPivot { get; }

        public Vector3 AimForwardLocal { get; }


        public Quaternion LocalRotation { get; private set; } =
            Quaternion.Identity;

        public override ComponentBase Clone()
        {
            return new UpperBodyComponent(PivotLocalPosition, AimOriginOffsetFromPivot, AimForwardLocal);
        }

        public UpperBodyComponent(
            Vector3 pivotLocalPosition,
            Vector3 aimOriginOffsetFromPivot,
            Vector3 aimForwardLocal)
        {
            if (aimForwardLocal.LengthSquared() < 0.000001f)
                throw new ArgumentException(
                    "AimForwardLocal cannot be zero.");

            PivotLocalPosition = pivotLocalPosition;
            AimOriginOffsetFromPivot = aimOriginOffsetFromPivot;
            AimForwardLocal = Vector3.Normalize(aimForwardLocal);
        }


        public void SetRotation(Quaternion localRotation)
        {
            if (localRotation.LengthSquared() < 0.000001f)
                throw new ArgumentException(
                    "UpperBody rotation cannot be zero.");

            LocalRotation = Quaternion.Normalize(localRotation);
        }


        public Vector3 GetWorldPivotPosition(
            Vector3 rootPosition,
            Quaternion rootRotation)
        {
            return rootPosition +
                   Vector3.Transform(
                       PivotLocalPosition,
                       rootRotation);
        }


        public Vector3 GetWorldAimOriginPosition(
            Vector3 rootPosition,
            Quaternion rootRotation)
        {
            Vector3 localPosition =
                PivotLocalPosition + AimOriginOffsetFromPivot;

            return rootPosition +
                   Vector3.Transform(localPosition, rootRotation);
        }


        public Quaternion GetWorldRotation(
            Quaternion rootRotation)
        {
            return Quaternion.Normalize(
                rootRotation * LocalRotation);
        }


        public Vector3 TransformPoint(
            Vector3 localPoint,
            Vector3 rootPosition,
            Quaternion rootRotation)
        {
            Vector3 pivot = GetWorldPivotPosition(
                rootPosition,
                rootRotation);

            Quaternion worldRotation =
                GetWorldRotation(rootRotation);

            return pivot +
                   Vector3.Transform(localPoint, worldRotation);
        }
    }
}
