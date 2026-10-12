using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model
{
    public class QueryExecutePlan<T>
    {
        private readonly QueryExecutionStep<T>[] _steps;
        private int _count;

        public int Count => _count;

        public ReadOnlySpan<QueryExecutionStep<T>> Steps
            => _steps.AsSpan(0, _count);

        public QueryExecutePlan(int capacity)
        {
            _steps = new QueryExecutionStep<T>[capacity];
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
