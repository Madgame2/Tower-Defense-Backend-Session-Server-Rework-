using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.StateMachine.StatesGraph;
using GameServer.Domain.Sessions.StateMachine.Factory;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Application.Sessions;


namespace GameServer.Domain.Sessions.StateMachine
{
    public class SessionStateMachine
    {
        private readonly StateGraph _stateGraph;
        private readonly GameSession _session;
        private readonly StateFactory _stateFactory;

        private BaseState _currentState;
        private bool _started = false;

        public SessionStateMachine(
            GameSession session,
            StateGraph graph,
            StateFactory stateFactory)
        {
            _session = session;
            _stateGraph = graph;
            _stateFactory = stateFactory;
        }

        public Task Publish(SessionEvent evt)
        {
            if (!_started)
                throw new InvalidOperationException("StateMachine not started");

            return _currentState.Handle(evt, _session);
        }

        public async Task MoveTo<TState>()
            where TState : BaseState
        {
            var from = _currentState.GetType();
            var to = typeof(TState);

            if (!_stateGraph.CanTransit(from, to))
            {
                throw new InvalidOperationException(
                    $"Invalid transition {from.Name} -> {to.Name}");
            }

            await _currentState.OnExit(_session);

            _currentState = _stateFactory.Create<TState>();

            await _currentState.OnEnter(_session);
        }

        public async Task Start()
        {
            if (_started)
                return;

            var rootType = _stateGraph.RootState;

            _currentState =
                _stateFactory.Create(rootType);

            await _currentState.OnEnter(_session);

            _started = true;
        }
    }
}
