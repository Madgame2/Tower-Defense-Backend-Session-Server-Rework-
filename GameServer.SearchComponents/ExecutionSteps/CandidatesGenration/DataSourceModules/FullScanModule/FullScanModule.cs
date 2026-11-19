using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule.MetaDatas;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using GameServer.SearchComponents.TypesTranslator;
using GameServer.SearchComponents.TypesTranslator.Builder.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule
{
    public class FullScanModule : IPossibilitySoureseModuel
    {
        private const int FULL_SCAN_COST = 100;

        private readonly IFullScanAdapterRegistry _adapterRegistry;
        private readonly ITranslatorBuilder _translator;

        public FullScanModule(
            IFullScanAdapterRegistry adapterRegistry,
            ITranslatorBuilder translator)
        {
            _adapterRegistry = adapterRegistry;
            _translator = translator;
        }
        public bool TryGetPossibilities<T>(
            IQueryOperation operation,
            QueryContext<T> queryContext,
            out List<PhysicalCandidate> possibilities)
        {
            possibilities = new List<PhysicalCandidate>();

            if (!_adapterRegistry.TryResolve<T>(operation, out var adapter))
                return false;

            Translator translator = null;
            try
            {
                translator = _translator.Create<T>(queryContext.ObjectregisterContext, adapter.RequriredType);
            }
            catch
            {
                return false;
            }

            possibilities.Add(new PhysicalCandidate(
                operation: operation,
                strategyType: ExecutionStrategyType.FullScan,
                role: ExecutionStrategyRole.Source,
                estimatedCost: FULL_SCAN_COST,
                metaData: new FullScanMetaData<T>
                {
                    Translator = translator,
                    Adapter = adapter
                }));

            return true;
        }
    }
}

