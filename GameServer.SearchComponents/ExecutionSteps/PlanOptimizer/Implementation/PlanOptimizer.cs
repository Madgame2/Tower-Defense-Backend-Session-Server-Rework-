using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Algorithms.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Implementation
{
    public class PlanOptimizer : IQueryPlanOptimizer
    {
        private readonly IOptimizeStrategy _optimizeAlgorithm;

        public PlanOptimizer(IOptimizeStrategy optimizeAlgorithm)
        {
            _optimizeAlgorithm = optimizeAlgorithm;
        }

        public IPlanNode<T> Optimize<T>(ICandidatesMatrix candidatesContainer, QueryContext<T> queryContext)
        {
            if (candidatesContainer is not CandidatesMatrix candidatesMatrix)
            {
                throw new ArgumentException("candidatesContainer is not type: CandidatesMatrix");
            }

            return _optimizeAlgorithm.Resolve<T>(candidatesMatrix);

        }
    }
}
