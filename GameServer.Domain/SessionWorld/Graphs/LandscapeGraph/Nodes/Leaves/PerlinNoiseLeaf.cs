using DotnetNoise;
using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Base;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Enums;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Leaves
{
    public class PerlinNoiseLeaf : GraphLeaveBase
    {
        private readonly DotnetNoise.FastNoise _noise = new();

        public float Frequency
        {
            get => _noise.Frequency;
            set => _noise.Frequency = value;
        }

        public PerlinNoiseLeaf()
        {
            _noise.Frequency = 0.05f;
        }

        public override NodeType Type => NodeType.PerlinNoiseNode;

        public override bool TryEvaluate(float x, float y, out float result)
        {
            float rawNoise = _noise.GetPerlin(x, y);
            result = (rawNoise + 1.0f) / 2.0f;

            return true;
        }

        public override ILandscapeGraphNode[] GetChildren()
        {
            return Array.Empty<ILandscapeGraphNode>();
        }

        public override NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback)
        {
            return new[]
        {
            new NodeParam
            {
                Param = ParamsType.Frequency,
                ValueType = ParamValueType.Float,
                FloatValue = Frequency
            }
        };
        }
    }
}
