using GameServer.Domain.SessionWorld.Graphs.Base.Enums;
using GameServer.Domain.SessionWorld.Graphs.Base.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.Base.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Graphs.Base
{
    public class CompareCondition<T> : ICondition where T: IComparable<T>
    {
        private readonly IGraphNode<T> _a;
        private readonly IGraphNode<T> _b;
        private readonly CompareMode _mode;
        private readonly IComparer<T> _comparer;

        public CompareCondition(IGraphNode<T> a, IGraphNode<T> b, CompareMode mode)
        {
            _a = a;
            _b = b;
            _mode = mode;
            _comparer = Comparer<T>.Default;
        }

        public IGraphNode[] GetConditionNodes() => new IGraphNode[] { _a, _b };

        public ConditionData GetMetaData(Func<IGraphNode, short> serializeCallback) => new ConditionData
        {
            Type = ConditionType.Compare,
            CompareMode = _mode,

            InputAId = serializeCallback(_a),
            InputBId = serializeCallback(_b)
        };

        public bool Pass(float x, float y)
        {
            T valA = _a.Evaluate(x, y);
            T valB = _b.Evaluate(x, y);

            int result = _comparer.Compare(valA, valB);

            return _mode switch
            {
                CompareMode.Equal => result == 0,
                CompareMode.NotEqual => result != 0,
                CompareMode.Greater => result > 0,
                CompareMode.GreaterOrEqual => result >= 0,
                CompareMode.Less => result < 0,
                CompareMode.LessOrEqual => result <= 0,
                _ => false
            };
        }
    }
}
