using GameServer.Application.Messaging;
using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System.Buffers;


namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine
{
    public class QueryPlaner : IQueryPlanner
    {
        private readonly IQueryAdapterRegistry _queryAdaptersRegistry;


        public QueryPlaner(IQueryAdapterRegistry queryCapabilityRegistry)
        {
            _queryAdaptersRegistry = queryCapabilityRegistry;
        }

        public QueryExecutePlan<T> Build<T>(WorldQuery<T> query, QueryContext queryContext)
        {

        }
    }
}
