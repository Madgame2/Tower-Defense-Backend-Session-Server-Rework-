using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces
{
    public interface IQueryExecutor
    {
        void Execute<T>(QueryExecutePlan executePlan, QueryContext queryContext, IList<T> buffer);
    }
}
