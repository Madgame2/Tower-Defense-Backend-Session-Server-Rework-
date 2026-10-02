using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine
{
    public class QueryExecuteEngine : IQueryExecutor
    {
        public void Execute<T>(QueryExecutePlan executePlan, QueryContext queryContext, IList<T> buffer)
        {
            throw new NotImplementedException();
        }

        private bool HasIndexes<T>(
            IIndexStorage indexes,
            out ReadOnlySpan<IIndex<T>> result)
        {
            result = indexes.Get<T>();

            return !result.IsEmpty;
        }
    }
}
