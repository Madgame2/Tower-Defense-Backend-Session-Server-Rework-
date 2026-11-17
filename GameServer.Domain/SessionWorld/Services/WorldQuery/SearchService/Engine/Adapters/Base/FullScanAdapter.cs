using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base
{
    public abstract class FullScanAdapter<TOperation, T> : IFullScanAdapter
        where TOperation : IQueryOperation
    {
        public Type OperationType => typeof(TOperation);

        public Type DataType => typeof(T);

        public abstract FullScanExecutorDelegate<T> CreateExecutor();

        FullScanExecutorDelegate<TData> IFullScanAdapter.CreateExecutor<TData>()
        {
            if (typeof(TData) != typeof(T))
                throw new InvalidOperationException(
                    $"Adapter supports {typeof(T)}, but received {typeof(TData)}");

            return (FullScanExecutorDelegate<TData>)(object)CreateExecutor();
        }
    }

    public delegate Vector3 PositionGetter<T>(in T item);
}
