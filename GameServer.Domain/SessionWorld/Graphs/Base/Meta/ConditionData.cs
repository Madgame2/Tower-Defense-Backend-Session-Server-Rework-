using GameServer.Domain.SessionWorld.Graphs.Base.Enums;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.Base.Meta
{
    [MessagePackObject]
    public struct ConditionData
    {
        [Key(0)] public ConditionType Type { get; set; }
        [Key(1)] public CompareMode CompareMode { get; set; }

        // Константные значения (для RangeCondition / ValueCompare)
        [Key(2)] public float MinValue { get; set; }
        [Key(3)] public float MaxValue { get; set; }
        [Key(4)] public string TargetValue { get; set; }

        // Идентификаторы нод-входов (для CompareCondition между нодами)
        [Key(5)] public int InputAId { get; set; }
        [Key(6)] public int InputBId { get; set; }
    }
}
