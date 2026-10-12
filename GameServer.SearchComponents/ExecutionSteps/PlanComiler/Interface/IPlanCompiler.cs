using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.Interface
{
    public interface IPlanCompiler
    {
        IPlanNode<T> Compile<T>(IPlanNode<T>  planRoot, QueryContext<T> queryContext);
    }
}
