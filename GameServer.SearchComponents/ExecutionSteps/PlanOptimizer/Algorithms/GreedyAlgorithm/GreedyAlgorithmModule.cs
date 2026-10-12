using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Algorithms.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.CombineModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Algorithms.GreedyAlgorithm
{
    public class GreedyAlgorithmModule : IOptimizeStrategy
    {

        private readonly List<IPlanCombiner> _combineModules;

        public GreedyAlgorithmModule(IEnumerable<IPlanCombiner> combineModules)
        {
            _combineModules = combineModules.ToList();
        }

        public PhysicalPlanNode<T>? Resolve<T>(CandidatesMatrix availableOperationsMatrix)
        {
            var leavs = BuildLeaveslayer<T>(availableOperationsMatrix);
            var root = BuildExecutionTree<T>(leavs);

            return root;
        }

        private PhysicalPlanNode<T>? BuildExecutionTree<T>(List<PhysicalPlanNode<T>> leaves)
        {
            if (leaves == null || leaves.Count == 0)
                return null;

            var allOperations = new HashSet<IQueryOperation>();
            for (int i = 0; i < leaves.Count; i++)
            {
                if (leaves[i].QueryOperation != null)
                {
                    allOperations.Add(leaves[i].QueryOperation);
                }
            }

            if (allOperations.Count == 1 || leaves.Count == 1)
            {
                PhysicalPlanNode<T>? bestPlan = null;
                float minCost = float.MaxValue;

                for (int i = 0; i < leaves.Count; i++)
                {
                    var leaf = leaves[i];

                    if (IsComplete(leaf, allOperations))
                    {
                        if (leaf.TotalCost < minCost)
                        {
                            minCost = leaf.TotalCost;
                            bestPlan = leaf;
                        }
                    }
                }

                return bestPlan;
            }

            var queue = new PriorityQueue<PhysicalPlanNode<T>, float>();

            for (int i = 0; i < leaves.Count; i++)
            {
                for (int j = i + 1; j < leaves.Count; j++)
                {
                    var node1 = leaves[i];
                    var node2 = leaves[j];

                    if (node1.QueryOperation != null && node1.QueryOperation == node2.QueryOperation)
                        continue;

                    foreach (var module in _combineModules)
                    {
                        if (module.TryResolve(node1, node2, out var newRoot))
                        {
                            queue.Enqueue(newRoot, newRoot.TotalCost);
                        }
                    }
                }
            }

            while (queue.TryDequeue(out var current, out _))
            {
                if (IsComplete(current, allOperations))
                    return current;

                foreach (var leaf in leaves)
                {
                    if (current.Covers(leaf))
                        continue;

                    foreach (var module in _combineModules)
                    {
                        if (module.TryResolve(current, leaf, out var newRoot))
                        {
                            queue.Enqueue(newRoot, newRoot.TotalCost);
                        }
                    }
                }
            }

            return null;
        }

        private bool IsComplete<T>(PhysicalPlanNode<T> current, HashSet<IQueryOperation> allOperations)
        {
            return current.UsedOperations.SetEquals(allOperations);
        }

        private List<PhysicalPlanNode<T>> BuildLeaveslayer<T>(CandidatesMatrix availableOperationsMatrix)
        {
            var result = new List<PhysicalPlanNode<T>>();
            foreach(var operationsList in availableOperationsMatrix.OperationsMatrix.Values)
            {
                foreach(var physicalCandidate in operationsList)
                {
                    var newPhysicalNode = new PhysicalPlanNode<T>(physicalCandidate.EstimatedCost, physicalCandidate.StrategyType);
                    newPhysicalNode.UsedOperations.Add(physicalCandidate.Operation);
                    newPhysicalNode.MetaData = physicalCandidate.MetaData;
                    newPhysicalNode.QueryOperation = physicalCandidate.Operation;
                    newPhysicalNode.State = physicalCandidate.MetaData;

                    result.Add(newPhysicalNode);
                }
            }

            return result;
        }
    }
}
