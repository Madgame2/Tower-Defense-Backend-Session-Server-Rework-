using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    public struct ChunkMetaDatasMessage : IServerMessage
    {
        public ServerAction Action => ServerAction.METADATA_CHUNK_SETTINGS;

        public Vector2 Pivot { get; set; }
        public int Size { get; set; }
    }
}
