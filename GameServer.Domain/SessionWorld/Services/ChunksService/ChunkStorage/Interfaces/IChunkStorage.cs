using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.ChunksService.ChunkStorage.Interfaces
{
    public interface IChunkStorage
    {
        event Action<Chank> OnChunkUnloaded;

        Chank Get(Vector2 pivot);
        void Save(Chank chunk);
    }
}
