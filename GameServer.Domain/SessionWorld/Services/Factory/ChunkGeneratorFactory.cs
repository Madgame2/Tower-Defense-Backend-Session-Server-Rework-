using GameServer.Domain.SessionWorld.Generators;
using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Factory
{
    public class ChunkGeneratorFactory : IChunkGeneratorFactory
    {
        private readonly IServiceProvider _provider;
        private readonly IChunksSettings _chunksSettings;

        public ChunkGeneratorFactory(IServiceProvider provider, IChunksSettings chunksSettings)
        {
            _provider = provider;
            _chunksSettings = chunksSettings;
        }

        public SessionChunkGenerator CreateForSession(ILandscapeGraphNode landscapeRoot, IBiomeGraphNode biomeRoot, IGraphNode<StaticTreeData> treeRoot)
        {
            return new SessionChunkGenerator(landscapeRoot, biomeRoot, treeRoot, _chunksSettings);
        }
    }
}
