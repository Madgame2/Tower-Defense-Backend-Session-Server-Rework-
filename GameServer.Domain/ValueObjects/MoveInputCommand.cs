using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.ValueObjects
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct MoveInputCommand
    {
        public readonly uint Tick;
        public readonly Vector3 MoveDirection;

        public MoveInputCommand(uint tick, Vector3 moveDirection)
        {
            Tick = tick;
            MoveDirection = moveDirection;
        }
    }
}
