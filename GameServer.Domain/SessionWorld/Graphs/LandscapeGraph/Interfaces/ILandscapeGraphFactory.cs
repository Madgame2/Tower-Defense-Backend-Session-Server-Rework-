using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Interfaces
{
    public interface ILandscapeGraphFactory
    {
        ILandscapeGraphNode Create(long seed);
    }
}
