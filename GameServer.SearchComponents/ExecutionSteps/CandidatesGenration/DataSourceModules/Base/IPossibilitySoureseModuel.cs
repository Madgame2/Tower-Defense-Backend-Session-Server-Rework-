using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base
{
    public interface IPossibilitySoureseModuel
    {
        bool TryGetPossibilities<T>(IQueryOperation operation, QueryContext<T> queryContext, out List<PhysicalCandidate> possibilities);
    }
}
