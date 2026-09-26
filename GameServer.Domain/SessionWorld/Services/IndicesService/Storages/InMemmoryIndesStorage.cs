using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Storages
{
    public class InMemmoryIndesStorage : IIndexStorage
    {
        private readonly Dictionary<Type, IIndex> _indexDictionary = new();

        public void Add<T, TQuery>(IIndex<T, TQuery> index)
        {
            var key = typeof(IIndex<T, TQuery>);

            if (!_indexDictionary.TryAdd(key, index))
            {
                throw new InvalidOperationException(
                    $"Индекс типа {key} уже зарегистрирован."
                );
            }
        }

        public IIndex<T, TQuery> Get<T, TQuery>()
        {
            var key = typeof(IIndex<T, TQuery>);

            if (!_indexDictionary.TryGetValue(key, out var index))
            {
                throw new InvalidOperationException(
                    $"Индекс типа {key} не зарегистрирован."
                );
            }

            return (IIndex<T, TQuery>)index;
        }
    }
}
