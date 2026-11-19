using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta
{
    public sealed class RoomServices
    {
        private readonly Dictionary<Type, object> _items = new Dictionary<Type, object>();

        public RoomServices Add<TService>(TService instance) where TService : class
        {
            _items[typeof(TService)] = instance;
            return this;
        }

        public TService Get<TService>() where TService : class
        {
            if (_items.TryGetValue(typeof(TService), out var v))
                return (TService)v;

            throw new InvalidOperationException($"RoomServices: {typeof(TService).Name} is not registered");
        }
    }
}
