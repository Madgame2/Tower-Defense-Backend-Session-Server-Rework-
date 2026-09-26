using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.TreeGraph.Leaves
{
    public class CreateTreeLeave : ITreeGraphNode
    {
        public NodeType Type => NodeType.CreateTreeNode;

        public StaticTreeData Evaluate(float x, float y)
        {
            return new StaticTreeData { LocalPosition = new Vector2(x, y) };
        }

        public IGraphNode[] GetChildren()
        {
            return Array.Empty<IGraphNode>();
        }

        public NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback)
        {
            return Array.Empty<NodeParam>();
        }
    }
}
