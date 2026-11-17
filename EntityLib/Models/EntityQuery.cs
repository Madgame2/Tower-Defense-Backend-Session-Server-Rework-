using EntityLib.Infrastructure;
using EntityLib.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityLib.Models
{
    public ref struct EntityQuery
    {
        private readonly EntityManagerService _entityManager;
        private readonly Dictionary<Type, IStash> _stashesStorage;
        private readonly Filter _filter;
        private readonly IStash _smallestStash;

        private int _index;

        public Entity Current { get; private set; }

        public EntityQuery GetEnumerator() => this;

        internal EntityQuery(
            EntityManagerService entityManager,
            Dictionary<Type, IStash> stashesStorage,
            Filter filter,
            IStash smallestStash)
        {
            _entityManager = entityManager;
            _stashesStorage = stashesStorage;
            _filter = filter;
            _smallestStash = smallestStash;

            _index = -1;
            Current = default;
        }

        public bool MoveNext()
        {
            // Нет ни одного подходящего Stash для перебора.
            if (_smallestStash == null)
                return false;

            var entityIds = _smallestStash.EntityIds;

            while (++_index < entityIds.Length)
            {
                Entity entity =
                    _entityManager.GetEntity(entityIds[_index]);

                if (!Matches(entity))
                    continue;

                Current = entity;
                return true;
            }

            return false;
        }

        private bool Matches(Entity entity)
        {
            // Проверяем обязательные компоненты.
            foreach (var stashType in _filter.Include)
            {
                if (!_stashesStorage.TryGetValue(stashType, out var stash))
                    return false;

                if (!stash.Has(entity))
                    return false;
            }

            // Проверяем исключённые компоненты.
            foreach (var stashType in _filter.Exclude)
            {
                if (_stashesStorage.TryGetValue(stashType, out var stash)
                    && stash.Has(entity))
                    return false;
            }

            return true;
        }
    }
}
