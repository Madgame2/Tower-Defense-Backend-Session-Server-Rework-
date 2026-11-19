using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Entry;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Builder
{
    public sealed class ObjectMiddlewareBuilder
    {
        private readonly List<MiddlewareEntry> _entries = new List<MiddlewareEntry>();

        /// <summary>Модуль с публичным конструктором без параметров.</summary>
        public ObjectMiddlewareBuilder Use<TModule>(Action<MiddlewareOptions>? configure = null)
            where TModule : IObjectMiddleware, new()
            => Use(_ => new TModule(), configure);

        /// <summary>Модуль, которому нужны сервисы комнаты.</summary>
        public ObjectMiddlewareBuilder Use(Func<RoomServices, IObjectMiddleware> factory,
                                           Action<MiddlewareOptions>? configure = null)
        {
            var options = new MiddlewareOptions();
            configure?.Invoke(options);
            _entries.Add(new MiddlewareEntry(factory, options.Types, options.Operations));
            return this;
        }

        public ObjectMiddlewareDescription Build() => new ObjectMiddlewareDescription(_entries.ToArray());
    }
}
