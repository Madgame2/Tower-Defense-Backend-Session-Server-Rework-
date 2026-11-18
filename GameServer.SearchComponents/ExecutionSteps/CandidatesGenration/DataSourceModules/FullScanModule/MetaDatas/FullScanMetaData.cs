using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.SearchComponents.TypesTranslator;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule.MetaDatas
{
    public class FullScanMetaData<T>
    {
        public Translator Translator { get; set; } 
        public IFullScanAdapter Adapter { get; set; }

        public FullScanExecutorDelegate<T> Executor { get; set; }
    }
}
