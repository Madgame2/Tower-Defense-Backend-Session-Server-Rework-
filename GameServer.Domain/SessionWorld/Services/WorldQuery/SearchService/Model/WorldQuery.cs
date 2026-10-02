using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model
{
    public class WorldQuery<T>
    {
        internal IReadOnlyList<IQueryOperation> Operations { get; }

        internal WorldQuery(
            IReadOnlyList<IQueryOperation> operations)
        {
            Operations = operations;
        }
    }
}
