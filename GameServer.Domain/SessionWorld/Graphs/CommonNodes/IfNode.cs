using GameServer.Domain.SessionWorld.Graphs.Base;
using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.Enums;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Nodes.MetaData;
using GameServer.Domain.SessionWorld.Graphs.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.CommonNodes
{
    public class IfNode<T> : IGraphNode<T>
    {
        private readonly ICondition _condition;
        public IGraphNode<T> ThenNode { get; }
        public IGraphNode<T> ElseNode { get; }

        public NodeType Type => NodeType.IfNode;


        public IfNode(ICondition condition, IGraphNode<T> thenNode, IGraphNode<T> elseNode)
        {
            _condition = condition;
            ThenNode = thenNode;
            ElseNode = elseNode;
        }

        public bool TryEvaluate(float x, float y, out T output)
        {
            output = default;

            if (_condition.Pass(x, y))
            {
                return ThenNode != null && ThenNode.TryEvaluate(x, y, out output);
            }

            if (ElseNode != null)
            {
                return ElseNode.TryEvaluate(x, y, out var output1);
            }

            return false;
        }

        public IGraphNode[] GetChildren()
        {
            return new IGraphNode[] { ThenNode, ElseNode };
        }

        public NodeParam[] GetParams(Func<IGraphNode, short> serializeCallback)
        {
            return new[]
            {
                new NodeParam
                {
                    Param = ParamsType.Condition,
                    ValueType = ParamValueType.Condition,
                    ConditionValue = _condition.GetMetaData(serializeCallback),
                    GenericType = DataTypeRegistry.GetEnum(typeof(T))
                }
            };
        }

    }
}
