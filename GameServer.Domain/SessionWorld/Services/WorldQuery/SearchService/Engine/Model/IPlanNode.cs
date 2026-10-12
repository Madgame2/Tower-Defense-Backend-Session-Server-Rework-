using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model
{
    public interface IPlanNode<T>
    {
        IEnumerable<IPlanNode<T>> Children { get; }

        object MetaData { get; }
        StepExecutorDelegate<T> Executor { get; }
    }
}
