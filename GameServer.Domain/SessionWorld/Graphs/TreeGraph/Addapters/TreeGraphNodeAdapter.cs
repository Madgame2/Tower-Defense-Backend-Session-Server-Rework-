using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.TreeGraph.Addapters
{
    public class TreeGraphNodeAdapter : ITreeGraphNode
    {
        private readonly IGraphNode<StaticTreeData> _innerNode;

        public TreeGraphNodeAdapter(IGraphNode<StaticTreeData> innerNode)
        {
            _innerNode = innerNode;
        }

        public NodeType Type => throw new NotImplementedException();

        public bool TryEvaluate(float x, float y, out StaticTreeData result)
        {
            throw new NotImplementedException();
        }

        public IGraphNode[] GetChildren()
        {
            throw new NotImplementedException();
        }

        public NodeParam[] GetParams()
        {
            throw new NotImplementedException();
        }

        public NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback)
        {
            throw new NotImplementedException();
        }
    }
}
