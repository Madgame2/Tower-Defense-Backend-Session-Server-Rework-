using GameServer.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.interfaces
{
    public interface IServerMessage
    {
        ServerAction Action { get; }
    }
}
