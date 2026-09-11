using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Player.Enums
{
    public enum MovementState: byte
    {
        Grounded,
        Jumping,
        Falling
    }
}
