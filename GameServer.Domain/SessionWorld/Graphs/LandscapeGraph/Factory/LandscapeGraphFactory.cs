using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Leaves;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Factory
{
    public class LandscapeGraphFactory : ILandscapeGraphFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public LandscapeGraphFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILandscapeGraphNode Create(long seed)
        {
            return new PerlinNoiseLeaf();
        }
    }
}
