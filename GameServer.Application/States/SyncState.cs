using GameServer.Domain.Interfaces;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.Events.Stateshandler;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes;

namespace GameServer.Application.Sessions.States
{
    [Transition(typeof(SimulationState))]
    public class SyncState: BaseState
    {
        private readonly ISyncService _syncService;

        public SyncState(ISyncService syncService)
        {
            _syncService = syncService;
        }

        public override void Configure(StateEventHandler handlers)
        {
            base.Configure(handlers);

            handlers.Register<PlayerConnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player join {evt.PlayerId}");
            });

            handlers.Register<PlayerDisconnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player leave {evt.PlayerId}");
            });
        }

        public override async Task OnEnter(GameSession ctx)
        {
            Console.WriteLine("ON Sysnc State");

            await _syncService.SyncSessionUsersAsync(ctx.Id);

            await ctx.StateMachine.MoveTo<SimulationState>();
        }
    }
}