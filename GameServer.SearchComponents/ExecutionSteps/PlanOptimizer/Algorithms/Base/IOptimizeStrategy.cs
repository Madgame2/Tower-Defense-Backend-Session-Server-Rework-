using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Algorithms.Base
{
    public interface IOptimizeStrategy
    {
        PhysicalPlanNode<T>? Resolve<T>(CandidatesMatrix availableOperationsMatrix);
    }
}
