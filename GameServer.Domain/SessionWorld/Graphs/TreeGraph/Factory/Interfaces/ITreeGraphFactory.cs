using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.TreeGraph.Factory.Interfaces
{
    public interface ITreeGraphFactory
    {
        IGraphNode<StaticTreeData> Create(long seed);
    }
}
