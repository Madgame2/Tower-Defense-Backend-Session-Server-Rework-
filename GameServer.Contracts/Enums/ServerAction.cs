using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GameServer.Contracts.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ServerAction
    {
        DEBUG_ACTION,

        UPD_TOKEN,

        PREPARE_FOR_SYNC,
        METADATA_CHUNK_SETTINGS,
        METADATA_WORLD_GENERATION_SETTINGS,
        METADATA_DECORATION_RULES,
        METADATA_PLAYER_INIT,
        METADATA_SYNC_DONE
    }
}
