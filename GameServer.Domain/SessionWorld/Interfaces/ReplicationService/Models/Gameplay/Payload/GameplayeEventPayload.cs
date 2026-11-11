using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload.Player;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Gameplay.Payload
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GameplayeEventPayload
    {
        [FieldOffset(0)]
        public PlayerGameplayEventPayload PlayerEvent;


        public int GetSize(ObjectType objectType)
        {
            return objectType switch
            {
                ObjectType.Player => PlayerGameplayEventPayload.ByteSize,

                _ => throw new NotSupportedException(
                    $"Unsupported object type: {objectType}")
            };
        }

        public int WriteTo(Span<byte> destination, ObjectType objectType)
        {
            return objectType switch
            {
                ObjectType.Player => PlayerEvent.WriteTo(destination),

                _ => throw new NotSupportedException(
                    $"Unsupported object type: {objectType}")
            };
        }
    }
}
