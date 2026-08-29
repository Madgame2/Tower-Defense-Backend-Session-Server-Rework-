using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Nodes.Leaves;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.BiomGraph.Factories
{
    public class BiomGraphFactory : IBiomGraphFactory
    {
        public IBiomeGraphNode Create(long seed)
        {
            return new GreenMeadowsLeave();
        }
    }
}
