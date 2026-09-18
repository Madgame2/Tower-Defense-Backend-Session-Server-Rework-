using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base
{
    public abstract class GraphLeaveBase : ILandscapeGraphNode
    {
        public abstract NodeType Type { get; }

        public abstract float Evaluate(float x, float y);

        public abstract IGraphNode[] GetChildren();

        public abstract NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback);
    }
}
