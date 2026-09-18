using GameServer.Domain.SessionWorld.Graphs.Base;
using GameServer.Domain.SessionWorld.Graphs.Base.Enums;
using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.CommonNodes;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Leaves;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph.Factory.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph.Leaves;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.TreeGraph.Factory
{
    public class TreeGraphFactory : ITreeGraphFactory
    {
        public IGraphNode<StaticTreeData> Create(long seed)
        {
            var createTreeNode = new CreateTreeLeave();
            var constNode = new ConstNode<float>(0.5f);
            var perlineNodise = new PerlinNoiseLeaf();
            var condition = new CompareCondition<float>(perlineNodise, constNode, CompareMode.Greater);

            var ifNode = new IfNode<StaticTreeData>(condition, createTreeNode, null);

            return ifNode;
        }
    }
}
