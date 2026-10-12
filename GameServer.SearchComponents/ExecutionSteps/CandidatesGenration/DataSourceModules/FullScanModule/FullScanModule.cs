using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule
{
    public class FullScanModule : IPossibilitySoureseModuel
    {
        private const int TEMP_ESTIMATE_COST = 100;


        public bool TryGetPossibilities<T>(IQueryOperation operation, QueryContext<T> queryContext, out List<PhysicalCandidate> possibilities)
        {
            possibilities = new List<PhysicalCandidate>();

            var candidate = new PhysicalCandidate(
                operation: operation,
                strategyType: ExecutionStrategyType.FullScan,
                estimatedCost: TEMP_ESTIMATE_COST);

            possibilities.Add(candidate);

            return true;
        }
    }
}
