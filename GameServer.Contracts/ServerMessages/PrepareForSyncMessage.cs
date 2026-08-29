using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    public struct PrepareForSyncMessage : IServerMessage
    {
        public ServerAction Action => ServerAction.PREPARE_FOR_SYNC;
    }
}
