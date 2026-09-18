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
        NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback);
    }


    public interface IGraphNode<out TOutput> : IGraphNode
    {
        TOutput Evaluate(float x, float y);
    }
}
