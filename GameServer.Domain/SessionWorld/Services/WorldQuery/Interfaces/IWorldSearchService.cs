using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Builder;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces
{
    public interface IWorldSearchService
    {
        WorldQueryBuilder<T> Search<T>();
        void ExecuteQuery<T>(WorldQuery<T> treeQuery, QueryContext queryContext, IList<T> buffer);
    }
}
