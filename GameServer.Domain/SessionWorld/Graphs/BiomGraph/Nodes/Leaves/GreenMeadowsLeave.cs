using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Meta.Enums;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Nodes.Base;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.BiomGraph.Nodes.Leaves
{
    public class GreenMeadowsLeave : LeaveBase
    {
        public override NodeType Type => NodeType.GreenMeadowsNode;

        public override BiomeType Evaluate(float x, float y) => BiomeType.GREEN_MEADOWS;

        public override NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback)
        {
            return Array.Empty<NodeParam>();
        }
    }
}
