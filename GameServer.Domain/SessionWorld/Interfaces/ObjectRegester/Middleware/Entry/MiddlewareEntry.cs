using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Entry
{
    public sealed class MiddlewareEntry
    {
        public MiddlewareEntry(Func<RoomServices, IObjectMiddleware> factory,
                               HashSet<Type>? types, ObjectOperation operations)
        {
            Factory = factory;
            Types = types;
            Operations = operations;
        }

        public Func<RoomServices, IObjectMiddleware> Factory { get; }
        public HashSet<Type>? Types { get; }
        public ObjectOperation Operations { get; }
    }
}
