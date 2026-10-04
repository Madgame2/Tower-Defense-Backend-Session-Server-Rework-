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
        public void Execute<T>(QueryExecutePlan<T> executePlan, QueryContext queryContext, IList<T> buffer)
        {
            buffer.Clear();
            foreach (var step in executePlan)
            {
                step.Executor(step.MetaData, queryContext, buffer);
            }
        }
    }
}
