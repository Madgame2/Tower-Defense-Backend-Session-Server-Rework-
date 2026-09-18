using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GameServer.Domain.SessionWorld.Graphs.Base.Meta
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConditionType
    {
        Compare,   // Сравнение со значением (A > B)
        Range,     // Проверка диапазонов (Min <= A <= Max)
        Expression // Если в будущем появится текстовое выражение
    }
}
