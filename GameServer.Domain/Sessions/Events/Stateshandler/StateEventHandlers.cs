using GameServer.Application.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Sessions.Events.Stateshandler
{
    public class StateEventHandler
    {
        private readonly Dictionary<Type, Func<GameSession, SessionEvent, Task>> _handlers = new();

        public void Register<TEvent>(Func<GameSession, TEvent, Task> handler) where TEvent : SessionEvent
        {
            _handlers[typeof(TEvent)] =
                (ctx, evt) =>
                    handler(ctx, (TEvent)evt);
        }

        public Task Handle(GameSession ctx, SessionEvent evt)
        {
            if (_handlers.TryGetValue(
                evt.GetType(),
                out var handler))
            {
                return handler(ctx, evt);
            }

            return Task.CompletedTask;
        }
    }
}
