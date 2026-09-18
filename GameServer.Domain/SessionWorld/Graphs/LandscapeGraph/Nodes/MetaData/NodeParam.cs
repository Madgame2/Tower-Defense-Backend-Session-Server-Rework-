using GameServer.Domain.SessionWorld.Graphs.Base.Meta;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Enums;
using MessagePack;


namespace GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData
{
    [MessagePackObject]
    public struct NodeParam
    {
        [Key(0)]
        public ParamsType Param { get; set; }

        [Key(1)] public int IntValue;
        [Key(2)] public float FloatValue;
        [Key(3)] public bool BoolValue;
        [Key(4)] public string StringValue;

        [Key(5)] public ParamValueType ValueType;

        [Key(6)] public ConditionData ConditionValue;
    }
}