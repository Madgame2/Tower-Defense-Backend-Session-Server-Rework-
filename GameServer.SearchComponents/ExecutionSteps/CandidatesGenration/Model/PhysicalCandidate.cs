using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model
{
    public class PhysicalCandidate
    {
        public IQueryOperation Operation { get; }
        public ExecutionStrategyType StrategyType { get; }
        public int EstimatedCost { get; }
        public object MetaData { get; }


        public PhysicalCandidate(
        IQueryOperation operation,
        ExecutionStrategyType strategyType,
        int estimatedCost,
        object metaData = null)
        {
            Operation = operation;
            StrategyType = strategyType;
            EstimatedCost = estimatedCost;
            MetaData = metaData;
        }
    }
}
