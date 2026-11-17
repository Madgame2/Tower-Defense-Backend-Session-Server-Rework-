using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.IndexSearchModule.MetaDatas;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.IndexSearchModule
{
    public class IndexSearchModule : IPossibilitySoureseModuel
    {
        private const int TEMP_INDEX_COST = 50;

        public readonly IQueryAdapterRegistry _queryAdapterRegistry;

        public IndexSearchModule(IQueryAdapterRegistry queryAdapterRegistry)
        {
            _queryAdapterRegistry = queryAdapterRegistry;
        }

        public bool TryGetPossibilities<T>(IQueryOperation operation, QueryContext<T> queryContext, out List<PhysicalCandidate> possibilities)
        {
            possibilities = new List<PhysicalCandidate>();
            var capabilitiesCount = _queryAdapterRegistry.GetCapabilities<T>(operation, out var indexQueryTypes, out var adapters);

            if (capabilitiesCount == 0)
                return false;

            for (int i = 0; i < capabilitiesCount; i++)
            {
                Type queryType = indexQueryTypes[i];
                IQueryAdapter adapter = adapters[i];

                ReadOnlySpan<IndexEntry<T>> availableIndexes = queryContext.Indexes.Get<T>(queryType);

                if (availableIndexes.IsEmpty)
                    continue;

                for (int j = 0; j < availableIndexes.Length; j++)
                {
                    ref readonly var indexEntry = ref availableIndexes[j];

                    if (indexEntry.Index.StoragedType != typeof(T))
                        continue;

                    var candidate = new PhysicalCandidate(
                        operation: operation,
                        strategyType: ExecutionStrategyType.IndexScan,
                        role: ExecutionStrategyRole.Source,
                        estimatedCost: TEMP_INDEX_COST,
                        metaData: new IndexMetaData<T>
                        {
                            Index = indexEntry.Index,
                            QueryAdapter = adapter,
                        }
                        );

                    possibilities.Add(candidate);
                }
            }

            return true;
        }
    }
}
