using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.UpperBody;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.GameLoop.Core.Systems.ResolveSystems
{
    public class PlayerUpperBodyResolveSystem : ITickable
    {
        private const float Epsilon = 0.000001f;


        public void OnAwake(GameRoom world)
        {

        }

        public async Task Tick(float delta, uint tick, GameRoom world)
        {
            var allPlayer = world.AllPlayers;

            foreach (var player in allPlayer)
            {

                if (!player.TryGetComponent<UpperBodyComponent>(out var component))
                    continue;

                if (!player.TryGetComponent<TransformComponent>(out var transformComponent))
                    continue;

                var lookAtPoint = player.LookAt;
                Vector3 rootPosition = transformComponent.Position;
                Quaternion rootRotation = Quaternion.Normalize(transformComponent.Rotation);

                Vector3 aimOrigin = component.GetWorldAimOriginPosition(rootPosition, rootRotation);

                Vector3 worldDirection = lookAtPoint - aimOrigin;

                if (worldDirection.LengthSquared() < Epsilon)
                    continue;

                worldDirection = Vector3.Normalize(worldDirection);

                Quaternion inverseRootRotation = Quaternion.Inverse(rootRotation);
                Vector3 localDirection = Vector3.Transform(worldDirection, inverseRootRotation);

                localDirection = Vector3.Normalize(localDirection);

                Quaternion upperBodyRotation = FromToRotation(component.AimForwardLocal, localDirection);

                component.SetRotation(upperBodyRotation);
            }
        }

        private Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            from = Vector3.Normalize(from);
            to = Vector3.Normalize(to);

            float dot = Math.Clamp(Vector3.Dot(from, to), -1f, 1f);

            if (dot > 0.999999f)
                return Quaternion.Identity;

            if (dot < -0.999999f)
            {
                Vector3 axis = Vector3.Cross(from, Vector3.UnitY);

                if (axis.LengthSquared() < Epsilon)
                    axis = Vector3.Cross(from, Vector3.UnitX);

                axis = Vector3.Normalize(axis);

                return Quaternion.CreateFromAxisAngle(axis, MathF.PI);
            }

            Vector3 cross = Vector3.Cross(from, to);

            Quaternion rotation = new Quaternion(
                cross.X,
                cross.Y,
                cross.Z,
                1f + dot);

            return Quaternion.Normalize(rotation);
        }
    }
}
