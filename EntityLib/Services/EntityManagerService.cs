using EntityLib.Infrastructure;
using EntityLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace EntityLib.Services
{
    internal class EntityManagerService
    {
        private int _nextId = 0;

        private readonly Queue<int> _freeIds = new Queue<int>();

        private readonly List<ushort> _generations = new List<ushort>();

        private readonly Dictionary<Type, IStash> _stashesStorage;

        public EntityManagerService(Dictionary<Type, IStash> stashesStorage)
        {
            _stashesStorage = stashesStorage;
        }

        internal Entity CreateEntity()
        {
            if (_freeIds.TryDequeue(out int id))
            {
                return new Entity(id, _generations[id]);
            }

            id = _nextId++;
            _generations.Add(1);
            return new Entity(id, 1);
        }

        internal void DestroyEntity(Entity entity)
        {
            if (!IsAlive(entity)) return;

            foreach (var stash in _stashesStorage.Values)
            {
                stash.Remove(entity);
            }


            int id = entity.Id;

            _generations[id]++;

            _freeIds.Enqueue(id);
        }

        internal bool IsAlive(Entity entity)
        {

            return entity.Id >= 0 &&
                   entity.Id < _generations.Count &&
                   _generations[entity.Id] == entity.Version;
        }

        internal Entity GetEntity(int id)
        {
            return new Entity(id, _generations[id]);
        }

        internal EntityQuery SelectEntities(
            Dictionary<Type, IStash> stashesStorage,
            Filter filter)
        {
            var smallestStash = GetSmallestStash(stashesStorage, filter);

            if (smallestStash == null)
                return new EntityQuery(this, stashesStorage, filter, null);

            return new EntityQuery(
                this,
                stashesStorage,
                filter,
                smallestStash);
        }

        private IStash GetSmallestStash(
            Dictionary<Type, IStash> stashesStorage,
            Filter filter)
        {
            IStash smallestStash = null;

            foreach (var stashType in filter.Include)
            {
                if (!stashesStorage.TryGetValue(stashType, out var stash))
                    return null;

                if (smallestStash == null || stash.Size < smallestStash.Size)
                    smallestStash = stash;
            }

            return smallestStash;
        }
    }
}
