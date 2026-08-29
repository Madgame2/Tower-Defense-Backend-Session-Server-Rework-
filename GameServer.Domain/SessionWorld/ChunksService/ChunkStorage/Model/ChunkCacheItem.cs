using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.ChunksService.ChunkStorage.Model
{
    internal class ChunkCacheItem
    {
        public Chank Chunk { get; }

        public DateTime LastAccessedAt { get; set; }

        public ChunkCacheItem(Chank chunk)
        {
            Chunk = chunk;
            LastAccessedAt = DateTime.UtcNow;
        }
    }
}
