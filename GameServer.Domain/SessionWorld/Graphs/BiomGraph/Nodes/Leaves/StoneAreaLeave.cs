using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Meta.Enums;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Nodes.Base;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.BiomGraph.Nodes.Leaves
{
    public class StoneAreaLeave : LeaveBase
    {
        public override NodeType Type => NodeType.StoneAreaNode;

        public override BiomeType Evaluate(float x, float y) => BiomeType.STONE_AREA;

        public override NodeParam[] GetParams()
        {
            return Array.Empty<NodeParam>();
        }
    }
}
