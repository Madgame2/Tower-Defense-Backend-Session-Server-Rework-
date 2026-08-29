using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base
{
    public interface ILandscapeGraphNode : IGraphNode
    {
        float Evaluate(float x, float y);
    }
}
