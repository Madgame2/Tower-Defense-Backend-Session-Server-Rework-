using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.ObjectRegister
{
    class InMemmoryObjectRegister : IObjectRegistry
    {
        private readonly Dictionary<Type, IObjectHandler> _handlers = new Dictionary<Type, IObjectHandler>();

        public void RegisterHandler<T>(IObjectHandler<T> handler)
        {
            var handlerType= typeof(T);

            if (_handlers.ContainsKey(handlerType))
                return;

            _handlers[handlerType] = handler;
        }

        public void Add<T>(T obj)
        {
            var handlerType = typeof(T);

            if( !_handlers.TryGetValue(handlerType, out var handler))
            {
                throw new ArgumentException($"not found handler for type {handlerType}");
            }

            ((IObjectHandler<T>)handler).Add(obj);
        }

        public void Change<T>(T obj)
        {
            var handlerType = typeof(T);

            if (!_handlers.TryGetValue(handlerType, out var handler))
            {
                throw new ArgumentException($"not found hndlaer for type {handlerType}");
            }

            ((IObjectHandler<T>)handler).Change(obj);
        }

        public void Remove<T>(T obj)
        {
            var handlerType = typeof(T);

            if (!_handlers.TryGetValue(handlerType, out var handler))
            {
                throw new ArgumentException($"not found hndlaer for type {handlerType}");
            }

            ((IObjectHandler<T>)handler).Remove(obj);
        }
    }
}
