using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Storages
{
    public sealed class InMemmoryIndexStorage : IIndexStorage
    {
        private readonly Dictionary<Type, object> _buckets = new();

        public void Add<T, TQuery>(IIndex<T, TQuery> index)
        {
            if (!_buckets.TryGetValue(typeof(T), out var value))
            {
                var bucket = new IndexBucket<T>();
                _buckets.Add(typeof(T), bucket);
                value = bucket;
            }

            var typedBucket = (IndexBucket<T>)value;

            IndexExecutorDelegate<T> executor = (idx, operation, context, buffer) =>
            {
                var typedIndex = (IIndex<T, TQuery>)idx;

                var typedQuery = (TQuery)operation;

                typedIndex.Get(typedQuery, buffer);
            };

            typedBucket.Add<TQuery>(new IndexEntry<T>(index, executor));
        }

        public ReadOnlySpan<IIndex<T>> Get<T>()
        {
            if (!_buckets.TryGetValue(typeof(T), out var value))
                return ReadOnlySpan<IIndex<T>>.Empty;

            var bucket = (IndexBucket<T>)value;

            return CollectionsMarshal.AsSpan(bucket.Indices);
        }

        public ReadOnlySpan<IndexEntry<T>> Get<T, TQuery>()
        {
            if (!_buckets.TryGetValue(typeof(T), out var value))
                return ReadOnlySpan<IndexEntry<T>>.Empty;

            var bucket = (IndexBucket<T>)value;

            return bucket.Get<TQuery>();
        }

        public ReadOnlySpan<IndexEntry<T>> Get<T>(Type queryType)
        {
            if (!_buckets.TryGetValue(typeof(T), out var value))
                return ReadOnlySpan<IndexEntry<T>>.Empty;

            var bucket = (IndexBucket<T>)value;

            return bucket.Get(queryType);
        }

        private sealed class IndexBucket<T>
        {
            public readonly List<IIndex<T>> Indices = new();

            private readonly Dictionary<Type, List<IndexEntry<T>>> _queryIndices = new();

            public void Add<TQuery>(IndexEntry<T> index)
            {
                Indices.Add(index.Index);

                if (!_queryIndices.TryGetValue(
                        typeof(TQuery),
                        out var indices))
                {
                    indices = new List<IndexEntry<T>>();
                    _queryIndices.Add(typeof(TQuery), indices);
                }

                indices.Add(index);
            }

            public ReadOnlySpan<IndexEntry<T>> Get<TQuery>()
            {
                if (!_queryIndices.TryGetValue(
                        typeof(TQuery),
                        out var indices))
                {
                    return ReadOnlySpan<IndexEntry<T>>.Empty;
                }

                return CollectionsMarshal.AsSpan(indices);
            }

            public ReadOnlySpan<IndexEntry<T>> Get(Type queryType)
            {
                if (!_queryIndices.TryGetValue(
                        queryType,
                        out var indices))
                {
                    return ReadOnlySpan<IndexEntry<T>>.Empty;
                }

                return CollectionsMarshal.AsSpan(indices);
            }
        }
    }
}
