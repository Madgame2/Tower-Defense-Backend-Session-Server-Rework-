using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces
{
    public interface IBiomGraphFactory
    {
        IBiomeGraphNode Create(long seed);
    }
}
