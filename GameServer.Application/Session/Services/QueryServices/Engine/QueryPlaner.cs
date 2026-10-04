using GameServer.Application.Messaging;
using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System.Buffers;


namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine
{
    public class QueryPlaner : IQueryPlanner
    {
        private readonly IQueryAdapterRegistry _queryAdaptersRegistry;


        public QueryPlaner(IQueryAdapterRegistry queryCapabilityRegistry)
        {
            _queryAdaptersRegistry = queryCapabilityRegistry;
        }

        public QueryExecutePlan<T> Build<T>(WorldQuery<T> query, QueryContext queryContext, QueryExecutionStep<T>[] planBuffer)
        {
            var pool = ArrayPool<QueryCandidate<T>>.Shared;
            var candidates = pool.Rent(16);


            var executionPlan = new QueryExecutePlan<T>(planBuffer);
            try
            {
                foreach (var operation in query.Operations)
                {
                    var count = FindCandidates(operation, queryContext, candidates);

                    var selected = SelectCandidate(candidates, count);

                    executionPlan.Add(selected);
                }
            }
            finally
            {
                pool.Return(candidates);
            }

            return executionPlan;
        }

        private QueryExecutionStep<T> SelectCandidate<T>(
            QueryCandidate<T>[] candidates,
            int count)
        {
            for (var i = 0; i < count; i++)
            {
                ref readonly var candidate = ref candidates[i];

                if (candidate.Kind == QueryCandidate<T>.QueryCandidateKind.Index)
                {
                    // У нас больше нет QueryType, мы просто передаем готовую операцию, индекс и делегат
                    return new QueryExecutionStep<T>(
                        candidate.Operation,
                        candidate.Index,
                        candidate.Executor);
                }
            }

            // Если индекс не подошел, fallback на FullScan (берем операцию из первого кандидата)
            var operation = candidates[0].Operation;
            return QueryExecutionStep<T>.FullScan(operation);
        }


        private int FindCandidates<T>(
            IQueryOperation operation,
            QueryContext context,
            QueryCandidate<T>[] candidates)
        {
            var count = 0;
            var queryTypes = _queryAdaptersRegistry.GetCapabilities<T>(operation);

            foreach (var queryType in queryTypes)
            {
                // Get<T> теперь возвращает ReadOnlySpan<IndexEntry<T>>
                var entries = context.Indexes.Get<T>(queryType);

                foreach (ref readonly var entry in entries)
                {
                    candidates[count++] = new QueryCandidate<T>(operation, entry);
                }
            }

            candidates[count++] = QueryCandidate<T>.FullScan(operation);
            return count;
        }

        public readonly struct QueryCandidate<T>
        {
            public readonly QueryCandidateKind Kind;

            public readonly IQueryOperation Operation; // Операция, для которой мы нашли кандидата
            public readonly IIndex<T> Index;
            public readonly IndexExecutorDelegate<T> Executor; // Тот самый быстрый делегат

            // Принимаем готовую связку (Индекс + Делегат) из Хранилища
            public QueryCandidate(
                IQueryOperation operation,
                IndexEntry<T> entry)
            {
                Kind = QueryCandidateKind.Index;
                Operation = operation;
                Index = entry.Index;
                Executor = entry.Executor;
            }

            private QueryCandidate(QueryCandidateKind kind, IQueryOperation operation)
            {
                Kind = kind;
                Operation = operation;
                Index = null!;
                Executor = null!;
            }

            public enum QueryCandidateKind
            {
                Index,
                FullScan
            }

            public static QueryCandidate<T> FullScan(IQueryOperation operation)
            {
                return new QueryCandidate<T>(QueryCandidateKind.FullScan, operation);
            }
        }
    }

}
