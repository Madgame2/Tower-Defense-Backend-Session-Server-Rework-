using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage.Interfaces;
using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage.Model;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.ChunksService.ChunkStorage
{
    public class InMemmoryChunkStorage : IChunkStorage
    {
        private readonly ConcurrentDictionary<Vector2, ChunkCacheItem> _chunks = new();
        public event Action<Chank> OnChunkUnloaded;


        private readonly TimeSpan _lifetime = TimeSpan.FromSeconds(60);

        public InMemmoryChunkStorage()
        {

        }

        public Chank Get(Vector2 pivot)
        {
            if (_chunks.TryGetValue(pivot, out var item))
            {
                item.LastAccessedAt = DateTime.UtcNow;
                return item.Chunk;
            }
            return null;
        }

        public void Save(Chank chunk)
        {
            _chunks.TryAdd(chunk.Pivot, new ChunkCacheItem(chunk));
        }

        public void CleanupOldChunks()
        {
            var now = DateTime.UtcNow;
            var keysToRemove = new List<Vector2>();

            foreach (var kvp in _chunks)
            {
                if (now - kvp.Value.LastAccessedAt > _lifetime)
                    keysToRemove.Add(kvp.Key);
            }

            foreach (var key in keysToRemove)
            {
                if (_chunks.TryRemove(key, out var removedItem))
                {
                    OnChunkUnloaded?.Invoke(removedItem.Chunk);
                }
            }
        }
    }
}
