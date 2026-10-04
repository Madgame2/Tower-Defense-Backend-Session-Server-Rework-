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
            foreach(var executeStep in executePlan)
            {
                switch (executeStep.Kind)
                {
                    case QueryExecutionStep<T>.QueryExecutionStepKind.Index:

                        var selectedIndex = typeof(IIndex<,>).MakeGenericType(typeof(T), executeStep.QueryType);

                        break;
                    case QueryExecutionStep<T>.QueryExecutionStepKind.FullScan:

                        break;
                }
            }
        }
    }
}
