using EntityLib.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EntityLib.Models
{
    public sealed class Stash<T> : IStash
        where T : struct, IComponent
    {
        private T[] _components = new T[128];
        private ushort[] _generations = new ushort[128];

        private readonly List<int> _entityIds = new();

        private int[] _entityIndices = new int[128];

        public int Size => _entityIds.Count;


        public bool Has(Entity entity)
        {
            return entity.Id < _generations.Length &&
                   _generations[entity.Id] == entity.Version;
        }

        public ref T Get(Entity entity)
        {
            return ref _components[entity.Id];
        }

        public void Set(Entity entity, in T component)
        {
            EnsureCapacity(entity.Id);

            if (!Has(entity))
            {
                int index = _entityIds.Count;

                _entityIds.Add(entity.Id);
                _entityIndices[entity.Id] = index;
            }

            _components[entity.Id] = component;
            _generations[entity.Id] = entity.Version;
        }

        public void Remove(Entity entity)
        {
            if (!Has(entity))
                return;

            int index = _entityIndices[entity.Id];
            int lastIndex = _entityIds.Count - 1;

            int lastEntityId = _entityIds[lastIndex];


            if (index != lastIndex)
            {
                _entityIds[index] = lastEntityId;
                _entityIndices[lastEntityId] = index;
            }

            _entityIds.RemoveAt(lastIndex);

            _generations[entity.Id] = 0;
            _components[entity.Id] = default;
        }

        public ReadOnlySpan<int> EntityIds =>
            CollectionsMarshal.AsSpan(_entityIds);

        private void EnsureCapacity(int id)
        {
            if (id < _components.Length)
                return;

            int newSize = Math.Max(
                id + 1,
                _components.Length * 2);

            Array.Resize(ref _components, newSize);
            Array.Resize(ref _generations, newSize);
            Array.Resize(ref _entityIndices, newSize);
        }
    }
}
