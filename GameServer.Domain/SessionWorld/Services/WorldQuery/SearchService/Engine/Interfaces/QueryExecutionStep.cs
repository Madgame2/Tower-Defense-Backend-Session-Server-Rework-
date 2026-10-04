using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces
{
    public readonly struct QueryExecutionStep<T>
    {
        public readonly QueryExecutionStepKind Kind;
        public readonly IQueryOperation Operation;
        public readonly IIndex<T> Index;
        public readonly IndexExecutorDelegate<T> Executor;

        public QueryExecutionStep(IQueryOperation operation, IndexEntry<T> entry)
        {
            Kind = QueryExecutionStepKind.Index;
            Operation = operation;
            Index = entry.Index;
            Executor = entry.Executor;
        }

        private QueryExecutionStep(QueryExecutionStepKind kind, IQueryOperation operation)
        {
            Kind = kind;
            Operation = operation;
            Index = null;
            Executor = null;
        }

        public static QueryExecutionStep<T> FullScan(IQueryOperation operation)
                => new(QueryExecutionStepKind.FullScan, operation);

        public enum QueryExecutionStepKind
        {
            Index,
            FullScan
        }
    }
}
