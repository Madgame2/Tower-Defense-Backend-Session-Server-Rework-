using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base
{
    public abstract class FullScanAdapter<T, TRequired> : IFullScanAdapter
        where T : IQueryOperation
    {
        public Type OperationType => typeof(T);

        public Type RequriredType => typeof(TRequired);

        public abstract ScanAdapterDelegate<T1, TRequired> CreateExecutor<T1>();

        public Delegate CreateExecutorUntyped<T1>()
        {
            return CreateExecutor<T1>();
        }
    }
}
