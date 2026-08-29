using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GameServer.Contracts.MessageFormats
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageFormat
    {
        Json,
        Binary
    }
}
