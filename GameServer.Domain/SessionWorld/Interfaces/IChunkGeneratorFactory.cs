using GameServer.Domain.SessionWorld.Generators;
using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface IChunkGeneratorFactory
    {
        public SessionChunkGenerator CreateForSession(ILandscapeGraphNode landscapeRoot, IBiomeGraphNode biomeRoot);
    }
}
