using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Storages
{
    public class InMemmoryIndesStorage : IIndexStorage
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

            typedBucket.Indices.Add(index);
        }

        public ReadOnlySpan<IIndex<T>> Get<T>()
        {
            if (!_buckets.TryGetValue(typeof(T), out var value))
                return ReadOnlySpan<IIndex<T>>.Empty;

            var bucket = (IndexBucket<T>)value;

            return CollectionsMarshal.AsSpan(bucket.Indices);
        }


        private sealed class IndexBucket<T>
        {
            public readonly List<IIndex<T>> Indices = new();
        }
    }
}
