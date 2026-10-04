using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model
{
    public struct QueryExecutePlan<T>
    {
        private readonly QueryExecutionStep<T>[] _steps;
        private int _count;

        public readonly int Count => _count;

        public readonly ReadOnlySpan<QueryExecutionStep<T>> Steps
            => _steps.AsSpan(0, _count);

        public QueryExecutePlan(QueryExecutionStep<T>[] buffer)
        {
            _steps = buffer;
            _count = 0;
        }

        public void Add(QueryExecutionStep<T> step)
        {
            _steps[_count++] = step;
        }

        public ReadOnlySpan<QueryExecutionStep<T>>.Enumerator GetEnumerator()
        {
            return Steps.GetEnumerator();
        }
    }
}
