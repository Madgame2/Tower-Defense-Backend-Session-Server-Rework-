using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine
{
    public class QueryPlaner : IQueryPlanner
    {
        public QueryExecutePlan Build<T>(WorldQuery<T> query, QueryContext queryContext)
        {
            return default;
        }
    }
}
