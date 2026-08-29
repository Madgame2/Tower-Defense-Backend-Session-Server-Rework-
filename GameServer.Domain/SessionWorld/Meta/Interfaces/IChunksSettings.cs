using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Meta.Interfaces
{
    public interface IChunksSettings
    {
        int ChunkSize { get; }
        public Vector2 Pivot { get; }
    }
}
