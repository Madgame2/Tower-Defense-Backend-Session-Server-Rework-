using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces
{
    public interface IBiomeGraphNode: IGraphNode
    {
        BiomeType Evaluate(float x, float y);
    }
}
