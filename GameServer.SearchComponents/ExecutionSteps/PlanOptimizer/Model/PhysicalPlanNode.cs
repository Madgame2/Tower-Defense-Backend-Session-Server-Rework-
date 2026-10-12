using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model
{
    public struct PhysicalPlanNode<T> : IPlanNode<T>
    {
        private PhysicalPlanNode<T>[]? _childNodes;
        private HashSet<IQueryOperation>? _usedOperations;

        public float TotalCost { get; set; }
        public ExecutionStrategyType ExecutionStrategy { get; set; }
        public IQueryOperation QueryOperation { get; set; }
        public SaveAtMode SaveAt { get; set; } = SaveAtMode.RessulSet;

        public StepExecutorDelegate<T> executionDelegate;

        public ReadOnlySpan<PhysicalPlanNode<T>> ChildrenSpan => _childNodes;

        public PhysicalPlanNode<T>[] ChildNodes
        {
            get => _childNodes ?? Array.Empty<PhysicalPlanNode<T>>();
            set => _childNodes = value;
        }
        public HashSet<IQueryOperation> UsedOperations => _usedOperations;

        IEnumerable <IPlanNode<T>> IPlanNode<T>.Children
        {
            get
            {
                if (_childNodes == null) yield break;
                for (int i = 0; i < _childNodes.Length; i++)
                {
                    yield return _childNodes[i];
                }
            }
        }


        public object MetaData {get; set;}

        public StepExecutorDelegate<T> Executor => executionDelegate;
        public object State { get; internal set; }

        public PhysicalPlanNode(float cost, ExecutionStrategyType executionStrategy)
        {
            _usedOperations = new();

            TotalCost = cost;
            ExecutionStrategy = executionStrategy;
            QueryOperation = null!;
            _childNodes = null;
        }

        public void AddChildren(ReadOnlySpan<PhysicalPlanNode<T>> children)
        {
            if (children.IsEmpty) return;

            int currentLength = _childNodes?.Length ?? 0;
            Array.Resize(ref _childNodes, currentLength + children.Length);
            children.CopyTo(_childNodes.AsSpan(currentLength));
        }

        public void AddChild(PhysicalPlanNode<T> child)
        {
            int currentLength = _childNodes?.Length ?? 0;
            Array.Resize(ref _childNodes, currentLength + 1);
            _childNodes[currentLength] = child;
        }

        internal bool Covers(PhysicalPlanNode<T> leaf)
        {
            return leaf.QueryOperation != null && UsedOperations.Contains(leaf.QueryOperation);
        }
    }
}
