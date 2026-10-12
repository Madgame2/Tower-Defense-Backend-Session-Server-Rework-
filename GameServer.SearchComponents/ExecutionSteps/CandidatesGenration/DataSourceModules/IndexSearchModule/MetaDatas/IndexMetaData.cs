using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.IndexSearchModule.MetaDatas
{
    public class IndexMetaData<T>
    {
        public IIndex<T> Index;
        public IQueryAdapter QueryAdapter;
        public IndexExecutorDelegate<T> Executor;
    }
}
