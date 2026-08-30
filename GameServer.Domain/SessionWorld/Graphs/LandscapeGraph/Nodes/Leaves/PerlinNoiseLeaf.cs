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

        public override float Evaluate(float x, float y)
        {
            float rawNoise = _noise.GetPerlin(x, y);
            float result = (rawNoise + 1.0f) / 2.0f;

            Console.WriteLine(
                $"Perlin: x={x}, y={y}, freq={Frequency}, raw={rawNoise}, result={result}");

            return result;
        }

        public override ILandscapeGraphNode[] GetChildren()
        {
            return Array.Empty<ILandscapeGraphNode>();
        }

        public override NodeParam[] GetParams()
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
