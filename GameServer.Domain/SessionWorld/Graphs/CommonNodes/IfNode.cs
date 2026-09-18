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

        public T Evaluate(float x, float y)
        {
            return _condition.Pass(x, y)
                ? ThenNode.Evaluate(x, y)
                : ElseNode.Evaluate(x, y);
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
                    ConditionValue = _condition.GetMetaData(serializeCallback)
                }
            };
        }

    }
}
