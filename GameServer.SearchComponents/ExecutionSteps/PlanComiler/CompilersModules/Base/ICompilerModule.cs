using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules.Base
{
    public interface ICompilerModule
    {
        ExecutionStrategyType StategyType {  get; }
        void Compile<T>(ref PhysicalPlanNode<T> node, QueryContext<T> queryContext);
    }
}
