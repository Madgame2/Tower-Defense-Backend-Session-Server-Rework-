using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta
{
    public sealed class MiddlewareOptions
    {
        internal HashSet<Type>? Types;
        internal ObjectOperation Operations = ObjectOperation.All;

        /// <summary>Модуль сработает только для объектов этого типа (можно вызывать несколько раз).</summary>
        public MiddlewareOptions OnlyFor<T>()
        {
            (Types ??= new HashSet<Type>()).Add(typeof(T));
            return this;
        }

        /// <summary>Модуль сработает только на эти операции.</summary>
        public MiddlewareOptions OnlyOn(ObjectOperation operations)
        {
            Operations = operations;
            return this;
        }
    }
}
