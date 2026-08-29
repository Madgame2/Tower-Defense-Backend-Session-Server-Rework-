using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.Base.Interfaces
{
    public interface IGraphNode
    {
        NodeType Type { get; }
        IGraphNode[] GetChildren();
        NodeParam[] GetParams();
    }
}
