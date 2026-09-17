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

            int id = entity.Id;

            _generations[id]++;

            _freeIds.Enqueue(id);

            // TODO: Здесь в будущем мы добавим удаление всех компонентов этой сущности
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

        internal IEnumerable<Entity> SelectEntites(Dictionary<Type, IStash> stashesStorage, Filter filter)
        {
            var smallestStashType = GetSmalestSthash(stashesStorage, filter);

            var smallestStash = stashesStorage[smallestStashType];

            foreach ( var entityId in smallestStash.EntityIds)
            {
                Entity entity = GetEntity(entityId);

                bool include = true;

                foreach (var stashType in filter.Include)
                {
                    var stash = stashesStorage[stashType];

                    if (!stash.Has(entity))
                    {
                        include = false;
                        break;
                    }
                }

                if (!include)
                    continue;

                bool exclude = false;

                foreach (var stashType in filter.Exclude)
                {
                    var stash = stashesStorage[stashType];

                    if (stash.Has(entity))
                    {
                        exclude = true;
                        break;
                    }
                }

                if (exclude)
                    continue;

                yield return entity;
            }
        }

        private Type GetSmalestSthash(Dictionary<Type, IStash> stashesStorage, Filter scanner)
        {
            int smallestSize = -1;
            Type smallestStash = null;

            foreach(var stashType in scanner.Include)
            {
                var stash = stashesStorage[stashType];
                if(smallestSize == -1)
                {
                    smallestStash = stashType;
                    smallestSize = stash.Size;
                }
                else if (stash.Size< smallestSize)
                {
                    smallestStash = stashType;
                    smallestSize = stash.Size;
                }
            }

            return smallestStash;
        }
    }
}
