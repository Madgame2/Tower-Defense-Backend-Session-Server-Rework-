using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule.MetaDatas;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule
{
    public class FullScanModule : IPossibilitySoureseModuel
    {
        private const int FULL_SCAN_COST = 100;

        private readonly IFullScanAdapterRegistry _adapterRegistry;

        public FullScanModule(
            IFullScanAdapterRegistry adapterRegistry)
        {
            _adapterRegistry = adapterRegistry;
        }
        public bool TryGetPossibilities<T>(
            IQueryOperation operation,
            QueryContext<T> queryContext,
            out List<PhysicalCandidate> possibilities)
        {
            possibilities = new List<PhysicalCandidate>();

            if (!_adapterRegistry.TryResolve<T>(operation, out var adapter))
                return false;

            possibilities.Add(new PhysicalCandidate(
                operation: operation,
                strategyType: ExecutionStrategyType.FullScan,
                role: ExecutionStrategyRole.Source,
                estimatedCost: FULL_SCAN_COST,
                metaData: new FullScanMetaData<T>
                {
                    Adapter = adapter
                }));

            return true;
        }
    }
}

