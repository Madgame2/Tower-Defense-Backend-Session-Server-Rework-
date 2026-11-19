using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Entry;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Pippline;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Context
{
    public sealed class ObjectMiddlewareDescription
    {
        private readonly MiddlewareEntry[] _entries;

        internal ObjectMiddlewareDescription(MiddlewareEntry[] entries) => _entries = entries;

        /// <summary>Создаёт НОВЫЕ экземпляры модулей под конкретную комнату.</summary>
        public ObjectMiddlewarePipeline Build(RoomServices services)
        {
            var slots = new ObjectMiddlewarePipeline.Slot[_entries.Length];

            for (int i = 0; i < _entries.Length; i++)
            {
                var e = _entries[i];
                slots[i] = new ObjectMiddlewarePipeline.Slot(e.Factory(services), e.Types, e.Operations);
            }

            return new ObjectMiddlewarePipeline(slots);
        }
    }
}
