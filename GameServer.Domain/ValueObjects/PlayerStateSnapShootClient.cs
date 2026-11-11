using GameServer.Domain.ValueObjects.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.ValueObjects
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PlayerStateSnapShootClient
    {
        public readonly uint Tick;
        public readonly Vector3 MoveDirection;
        public readonly Quaternion Rotation;
        public readonly Vector3 LookAtPoint;
        public readonly ActionFlags Actions;

        public PlayerStateSnapShootClient(uint tick, Vector3 moveDirection, Quaternion rotation, Vector3 lookAt, ActionFlags flags)
        {
            Tick = tick;
            MoveDirection = moveDirection;
            Rotation = rotation;
            LookAtPoint = lookAt;
            Actions = flags;
        }
    }
}
