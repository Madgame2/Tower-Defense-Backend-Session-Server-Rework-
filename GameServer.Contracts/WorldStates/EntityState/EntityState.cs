using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GameServer.Contracts.WorldStates.EntityState.Projectile;
using System.Numerics;

namespace GameServer.Contracts.WorldStates.EntityState
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EntityState
    {
        [FieldOffset(0)]
        public ObjectType objectType;

        [FieldOffset(1)]
        public ProjectileState projectile;

        public readonly int GetSize()
        {
            return 1 + (objectType switch
            {
                ObjectType.Projectile => projectile.GetSize(),
                _ => throw new NotSupportedException(
                    $"Unsupported entity type: {objectType}")
            });
        }

        public readonly int WriteTo(Span<byte> destination)
        {
            destination[0] = (byte)objectType;

            return 1 + (objectType switch
            {
                ObjectType.Projectile => projectile.WriteTo(destination.Slice(1)),
                _ => throw new NotSupportedException(
                    $"Unsupported entity type: {objectType}")
            });
        }

        public static EntityState ProjectileSnapshot(
            uint objectID,
            Vector3 position,
            Quaternion rotation)
        {
            return new EntityState
            {
                objectType = ObjectType.Projectile,
                projectile = new ProjectileState
                {
                    ObjectId = objectID,
                    Position = position,
                    Rotation = rotation
                }
            };
        }
    }
}
