using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.CombineModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.CombineModules.MemmoryFiletModule
{
    internal class MemmoryFilter : IPlanCombiner
    {
        public bool TryResolve<T>(PhysicalPlanNode<T> pysicalOperatoin_1, PhysicalPlanNode<T> pysicalOperatoin_2, out PhysicalPlanNode<T> newPhysicalNode)
        {
            newPhysicalNode = default;
            if (!CanResolve(pysicalOperatoin_1, pysicalOperatoin_2))
            {
                return false;
            }

            newPhysicalNode = new PhysicalPlanNode<T>(
                pysicalOperatoin_1.TotalCost + pysicalOperatoin_2.TotalCost,
                Enums.ExecutionStrategyType.MemmoryFilter);

            newPhysicalNode.UsedOperations.UnionWith(pysicalOperatoin_1.UsedOperations);
            newPhysicalNode.UsedOperations.UnionWith(pysicalOperatoin_2.UsedOperations);

            pysicalOperatoin_1.SaveAt = Model.Enums.SaveAtMode.RessulSet;

            newPhysicalNode.AddChild(pysicalOperatoin_1);

            return true;
        }

        private bool CanResolve<T>(PhysicalPlanNode<T> pysicalOperatoin_1, PhysicalPlanNode<T> pysicalOperatoin_2)
        {
            return
                (pysicalOperatoin_1.ExecutionStrategy == Enums.ExecutionStrategyType.FullScan ||
                pysicalOperatoin_1.ExecutionStrategy == Enums.ExecutionStrategyType.IndexScan||
                pysicalOperatoin_1.ExecutionStrategy == Enums.ExecutionStrategyType.MemmoryFilter) &&
                pysicalOperatoin_2.ExecutionStrategy == Enums.ExecutionStrategyType.MemmoryFilter;
        }
    }
}
