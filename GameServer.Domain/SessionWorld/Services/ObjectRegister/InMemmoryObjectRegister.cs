using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Context;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Delegates;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Pippline;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.ObjectRegister
{
    public class InMemmoryObjectRegister : IObjectRegistry
    {
        private const int MaxDepth = 16;

        private readonly ObjectMiddlewarePipeline _pipeline;
        private readonly Dictionary<Type, IObjectHandler> _handlers = new Dictionary<Type, IObjectHandler>();

        private readonly Dictionary<(Type, ObjectOperation), Delegate> _chains
        = new Dictionary<(Type, ObjectOperation), Delegate>();

        private int _depth;

        public InMemmoryObjectRegister(ObjectMiddlewarePipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public void RegisterHandler(IObjectHandler handler)
        {
            var handlerType= handler.StoragedType;

            if (_handlers.ContainsKey(handlerType))
                return;

            _handlers[handlerType] = handler;
        }


        public void Add<T>(T obj) => Run(ObjectOperation.Add, obj);
        public void Change<T>(T obj) => Run(ObjectOperation.Change, obj);
        public void Remove<T>(T obj) => Run(ObjectOperation.Remove, obj);

        public TResult? Get<TResult, TSearch>(TSearch arg)
        {
            var handlerType = typeof(TResult);

            if (!_handlers.TryGetValue(handlerType, out var handler))
            {
                throw new ArgumentException(
                    $"Not found handler for type {handlerType}");
            }

            if (handler is not IObjectHandler<TResult, TSearch> typedHandler)
            {
                throw new InvalidOperationException(
                    $"Handler for {handlerType} does not support search type {typeof(TSearch)}");
            }

            return typedHandler.Get(arg);
        }

        public ReadOnlySpan<T> GetAll<T>()
        {
            var handlerType = typeof(T);

            if (!_handlers.TryGetValue(handlerType, out var handler))
            {
                throw new ArgumentException(
                    $"Not found handler for type {handlerType}");
            }


            return ((IObjectHandler<T>)handler).GetAll();
        }

        private void Run<T>(ObjectOperation op, T obj)
        {
            if (_depth >= MaxDepth)
                throw new InvalidOperationException(
                    $"Middleware recursion limit ({MaxDepth}) exceeded on {op} {typeof(T)}");

            var chain = GetChain<T>(op);

            _depth++;
            try
            {
                chain(new ObjectContext<T>(op, obj, this, _depth));
            }
            finally
            {
                _depth--;
            }
        }

        private ObjectDelegate<T> GetChain<T>(ObjectOperation op)
        {
            var key = (typeof(T), op);
            if (_chains.TryGetValue(key, out var cached))
                return (ObjectDelegate<T>)cached;

            var handler = GetHandler<T>();

            ObjectDelegate<T> terminal;
            if (op == ObjectOperation.Add)
                terminal = ctx => handler.Add(ctx.Object);
            else if (op == ObjectOperation.Change)
                terminal = ctx => handler.Change(ctx.Object);
            else
                terminal = ctx => handler.Remove(ctx.Object);

            var chain = _pipeline.Build(op, terminal);
            _chains[key] = chain;
            return chain;
        }

        private IObjectHandler<T> GetHandler<T>()
        {
            if (!_handlers.TryGetValue(typeof(T), out var handler))
                throw new ArgumentException($"Not found handler for type {typeof(T)}");

            return (IObjectHandler<T>)handler;
        }
    }
}
