using GameServer.Domain.Sessions.Events.Stateshandler;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Domain.Sessions.StateMachine.Factory
{
    public class StateFactory
    {
        private readonly IServiceProvider _provider;

        public StateFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public BaseState Create(Type type)
        {
            var state = (BaseState)ActivatorUtilities.CreateInstance(_provider, type);

            var handlers = new StateEventHandler();
            state.Configure(handlers);

            return state;
        }

        public T Create<T>() where T : BaseState
        {
            var state = ActivatorUtilities.CreateInstance<T>(_provider);

            var handlers = new StateEventHandler();
            state.Configure(handlers);

            return state;
        }
    }
}
