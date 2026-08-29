using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    public struct SyncMetadataDoneMessage : IServerMessage
    {
        public ServerAction Action => ServerAction.METADATA_SYNC_DONE;
    }
}
