using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GameServer.Domain.SessionWorld.Graphs.Meta.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NodeType
    {
        NONE,
        PerlinNoiseNode,

        GreenMeadowsNode,
        StoneAreaNode
    }
}
