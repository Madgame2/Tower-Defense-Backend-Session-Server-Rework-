using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base
{
    public interface IQueryAdapter
    {
        Type From { get; }
        Type To { get; }

        IndexExecutorDelegate<T> CreateExecutor<T>(IIndex<T> index);
    }
}
