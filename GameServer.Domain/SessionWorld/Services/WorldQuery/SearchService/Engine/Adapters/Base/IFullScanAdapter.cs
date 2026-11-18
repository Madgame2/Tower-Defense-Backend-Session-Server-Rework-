using GameServer.Application.Messaging;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base
{
    public interface IFullScanAdapter
    {
        Type OperationType { get; }
        Type RequriredType { get; }

        Delegate CreateExecutorUntyped<T>();
    }

    public delegate bool ScanAdapterDelegate<T,TReq>(TReq req, IQueryOperation operation, QueryContext<T> context);
}
