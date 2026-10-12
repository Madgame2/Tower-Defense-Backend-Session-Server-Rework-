using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces
{
    public interface IQueryAdapterRegistry
    {
        ReadOnlySpan<Type> GetCapabilities<T>(IQueryOperation operation);

        int GetCapabilities<T>(IQueryOperation operation, out ReadOnlySpan<Type> indexQuerisTypes, out ReadOnlySpan<IQueryAdapter> adapters);
    }
}
