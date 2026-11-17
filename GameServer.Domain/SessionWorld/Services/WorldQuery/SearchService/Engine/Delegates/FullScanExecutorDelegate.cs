using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates
{
    public delegate void FullScanExecutorDelegate<T>(
        IQueryOperation operation,
        QueryContext<T> context,
        ReadOnlySpan<T> source,
        List<T> buffer);
}
