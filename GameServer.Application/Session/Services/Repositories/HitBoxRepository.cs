using GameServer.Application.Components.GameObjectsComponents;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Application.Session.Services.Repositories
{
    public sealed class HitBoxRepository
    {
        private readonly List<HitBoxComponent> _items = new();

        public void Add(HitBoxComponent item)
        {
            _items.Add(item);
        }

        public bool Remove(HitBoxComponent item)
        {
            return _items.Remove(item);
        }

        public ReadOnlySpan<HitBoxComponent> GetAll()
        {
            return CollectionsMarshal.AsSpan(_items);
        }

        public int Count => _items.Count;
    }
}
