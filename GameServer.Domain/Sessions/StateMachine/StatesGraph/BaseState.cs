using GameServer.Application.Sessions;
using GameServer.Domain.Sessions;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.Events.Stateshandler;

namespace GameServer.Domain.Sessions.StateMachine
{
    public abstract class BaseState
    {

        private StateEventHandler _handlers;

        public virtual void Configure(StateEventHandler handlers)
        {
            _handlers = handlers;
        }

        public virtual Task OnEnter(GameSession ctx)
        => Task.CompletedTask;

        public virtual Task OnExit(
            GameSession ctx)
                => Task.CompletedTask;

        public Task Handle(SessionEvent evt, GameSession ctx)
        {
            if (_handlers == null)
                return Task.CompletedTask;

            return _handlers.Handle(ctx, evt);
        }

    }
}
