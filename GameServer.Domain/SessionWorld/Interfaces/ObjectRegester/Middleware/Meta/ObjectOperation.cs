using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta
{
    [Flags]
    public enum ObjectOperation
    {
        Add = 1,
        Change = 2,
        Remove = 4,
        All = Add | Change | Remove
    }
}
