using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Enums;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.CommonNodes
{
    public class ConstNode<T> : IGraphNode<T> where T: INumber<T>
    {
        public T _const;

        public NodeType Type => NodeType.ConstNode;

        public ConstNode(T value)
        {
            _const = value;
        }

        public T Evaluate(float x, float y)
        {
            return _const;
        }

        public IGraphNode[] GetChildren()
        {
            return Array.Empty<IGraphNode>();
        }

        public NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback = null)
        {
            var param = new NodeParam
            {
                Param = ParamsType.ConstValue
            };

            switch (_const)
            {
                case float f:
                    param.ValueType = ParamValueType.Float;
                    param.FloatValue = f;
                    break;

                case int i:
                    param.ValueType = ParamValueType.Int;
                    param.IntValue = i;
                    break;

                case double d:
                    param.ValueType = ParamValueType.Float;
                    param.FloatValue = (float)d;
                    break;

                case long l:
                    param.ValueType = ParamValueType.Int;
                    param.IntValue = (int)l;
                    break;

                case byte b:
                    param.ValueType = ParamValueType.Int;
                    param.IntValue = b;
                    break;

                case short s:
                    param.ValueType = ParamValueType.Int;
                    param.IntValue = s;
                    break;

                case bool bVal:
                    param.ValueType = ParamValueType.Bool;
                    param.BoolValue = bVal;
                    break;

                case string sVal:
                    param.ValueType = ParamValueType.String;
                    param.StringValue = sVal;
                    break;

                default:
                    param.ValueType = ParamValueType.Float;
                    param.FloatValue = Convert.ToSingle(_const);
                    break;
            }

            return new[] { param };
        }
    }
}
