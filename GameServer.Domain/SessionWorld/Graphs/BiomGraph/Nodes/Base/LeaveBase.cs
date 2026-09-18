using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Meta.Enums;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.BiomGraph.Nodes.Base
{
    public abstract class LeaveBase : IBiomeGraphNode
    {
        public abstract NodeType Type { get; }

        public IGraphNode[] GetChildren()
        {
            return Array.Empty<IBiomeGraphNode>();
        }
        public abstract BiomeType Evaluate(float x, float y);

        public abstract NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback);
    }
}
