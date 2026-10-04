using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces
{
    public readonly struct QueryExecutionStep<T>
    {
        public readonly object MetaData;
        public readonly StepExecutorDelegate<T> Executor;

        public QueryExecutionStep(object metaData, StepExecutorDelegate<T> executor)
        {
            MetaData = metaData;
            Executor = executor;
        }
    }
}
