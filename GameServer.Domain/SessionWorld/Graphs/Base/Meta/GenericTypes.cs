using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GameServer.Domain.SessionWorld.Graphs.Base.Meta
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GenericTypes
    {
        None,
        String,
        Int,
        Float,
        Bool,
        StaticTreeData
    }
}
