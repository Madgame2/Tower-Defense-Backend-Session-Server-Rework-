using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ParamsType
    {
        Frequency
    }
}
