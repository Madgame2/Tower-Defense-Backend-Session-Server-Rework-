using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Context
{
    public struct ObjectContext<T>
    {
        public ObjectContext(ObjectOperation operation, T obj, IObjectRegistry registry, int depth)
        {
            Operation = operation;
            Object = obj;
            Registry = registry;
            Depth = depth;
        }

        public ObjectOperation Operation { get; }
        public T Object { get; }

        /// <summary>
        /// Реестр ЭТОЙ комнаты. Приходит в контексте, а не в конструкторе модуля —
        /// именно это убирает циклическую зависимость Registry <-> Middleware.
        /// </summary>
        public IObjectRegistry Registry { get; }

        /// <summary>Глубина вложенных вызовов реестра (защита от бесконечной рекурсии).</summary>
        public int Depth { get; }
    }
}
