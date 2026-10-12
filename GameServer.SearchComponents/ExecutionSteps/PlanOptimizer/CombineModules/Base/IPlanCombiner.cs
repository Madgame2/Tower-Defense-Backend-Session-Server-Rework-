using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.CombineModules.Base
{
    public interface IPlanCombiner
    {
        bool TryResolve<T>(PhysicalPlanNode<T> pysicalOperatoin_1, PhysicalPlanNode<T> pysicalOperatoin_2, out PhysicalPlanNode<T> newPhysicalNode);
    }
}
