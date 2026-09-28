using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.ObjectRegister.Handlers
{
    public class ChunkRegisterHandler : IObjectHandler<Chank>
    {
        private readonly IChunkStorage _chunkStorage;
        private readonly IIndexStorage _indexStorage;

        public ChunkRegisterHandler(IChunkStorage chunkStorage, IIndexStorage indexStorage)
        {
            _chunkStorage = chunkStorage;
            _indexStorage = indexStorage;
        }

        public void Add(Chank chunk)
        {
            _chunkStorage.Save(chunk);

            RegisterObjects(chunk);
        }

        public void Change(Chank obj)
        {
            throw new NotImplementedException();
        }

        public void Remove(Chank obj)
        {
            throw new NotImplementedException();
        }

        private void RegisterObjects(Chank chunk)
        {
            foreach (var tree in chunk.Trees)
            {
                RegisterTree(tree);
            }
        }

        private void RegisterTree(StaticTreeData tree)
        {
            var indices = _indexStorage.Get<StaticTreeData>();
            foreach (var index in indices)
            {
                index.Add(tree);
            }
        }
    }
}
