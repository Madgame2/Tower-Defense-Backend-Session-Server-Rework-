using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces
{
    public interface IFullScanAdapterRegistry
    {
        bool TryResolve<T>(
            IQueryOperation operation,
            out IFullScanAdapter adapter);
    }
}
