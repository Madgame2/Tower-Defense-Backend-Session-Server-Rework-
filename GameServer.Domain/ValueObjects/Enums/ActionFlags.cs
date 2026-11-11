using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.ValueObjects.Enums
{
    [Flags]
    public enum ActionFlags : byte
    {
        None = 0,
        JumpRequested = 1 << 0,
        AlternativeAction = 1 << 1
    }
}
