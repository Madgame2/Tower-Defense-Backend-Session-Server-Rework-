using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces
{
    public interface IQueryPlanner
    {
        QueryExecutePlan<T> Build<T>(WorldQuery<T> query, QueryContext queryContext, QueryExecutionStep<T>[] planBuffer);
    }
}
