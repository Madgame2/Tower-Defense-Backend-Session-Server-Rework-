using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.WorldMetadata
{
    [MessagePackObject]
    public struct GraphNodeDTO
    {
        [Key(0)]
        public NodeType Type { get; set;  }

        [Key(1)]
        public short ParentNode { get; set; } = -1;

        [Key(2)]
        public short[] ChildNodes { get; set; }

        [Key(3)]
        public NodeParam[] Params{ get; set; }

        public GraphNodeDTO()
        {
            ParentNode = -1;
        }

        public GraphNodeDTO(NodeType type, short[] childNodes, NodeParam[] param)
        {
            Type = type;
            ParentNode = -1;
            ChildNodes = childNodes;
            Params = param;
        }

        public GraphNodeDTO(NodeType type, short parrentNode, short[] childNodes, NodeParam[] param)
        {
            Type = type;
            ParentNode = parrentNode;
            ChildNodes = childNodes;
            Params = param;
        }
    }
}
