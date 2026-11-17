using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base
{
    public interface IFullScanAdapter
    {
        Type OperationType { get; }
        Type DataType { get; }

        FullScanExecutorDelegate<T> CreateExecutor<T>();
    }
}
