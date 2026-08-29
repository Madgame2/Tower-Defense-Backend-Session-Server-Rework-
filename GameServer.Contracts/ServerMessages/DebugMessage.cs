using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    public class DebugMessage : IServerMessage
    {
        public ServerAction Action => ServerAction.DEBUG_ACTION;
    }
}
