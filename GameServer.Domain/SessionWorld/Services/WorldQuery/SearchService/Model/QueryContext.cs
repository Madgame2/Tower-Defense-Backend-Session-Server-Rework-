using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model
{
    public class QueryContext
    {
        private readonly IIndexStorage _indexStorage;

        public IIndexStorage Indexes => _indexStorage;

        public QueryContext(IIndexStorage indexStorage)
        {
            _indexStorage = indexStorage;
        }

        public void Set<T>(string argName, T Value)
        {
           
        }

        internal void Link(IQueryOperation operation, IEnumerable<IQueryParameter> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
