using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Delegates;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Pippline
{
    public sealed class ObjectMiddlewarePipeline
    {
        internal readonly struct Slot
        {
            public Slot(IObjectMiddleware module, HashSet<Type>? types, ObjectOperation operations)
            {
                Module = module;
                Types = types;
                Operations = operations;
            }

            public IObjectMiddleware Module { get; }
            private HashSet<Type>? Types { get; }
            private ObjectOperation Operations { get; }

            public bool Matches(Type type, ObjectOperation op)
                => (Operations & op) != 0 && (Types == null || Types.Contains(type));
        }

        private readonly Slot[] _slots;

        internal ObjectMiddlewarePipeline(Slot[] slots) => _slots = slots;

        /// <summary>
        /// Порядок вызова = порядок в конфиге. terminal — реальная работа хендлера в самом конце.
        /// </summary>
        public ObjectDelegate<T> Build<T>(ObjectOperation op, ObjectDelegate<T> terminal)
        {
            var next = terminal;

            for (int i = _slots.Length - 1; i >= 0; i--)
            {
                var slot = _slots[i];
                if (!slot.Matches(typeof(T), op))
                    continue;

                var module = slot.Module;
                var inner = next;
                next = ctx => module.Invoke(ctx, inner);
            }

            return next;
        }
    }
}
