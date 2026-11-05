using GameServer.Domain.ColliderSystem.Enum;
using GameServer.Domain.Interfaces;
using GameServer.Domain.Player;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.Events.Stateshandler;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes;
using System.Numerics;

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
                Console.WriteLine($"Player join {evt.UserId}");

                //await _syncService.SyncPlayerAsync(ctx.Id, evt.PlayerId);
            });

            handlers.Register<PlayerDisconnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player leave {evt.UserId}");
            });
        }

        public override async Task OnEnter(GameSession ctx)
        {
            Console.WriteLine("ON Sysnc State");

            var allClients = ctx.OnlinePlayersIDs;

            ctx.SyncBarrier.Begin(allClients);

            bool completed = await ctx.SyncBarrier.WaitAsync(TimeSpan.FromSeconds(60), CancellationToken.None);

            if (!completed)
            {
                Console.WriteLine("Synchronization timeout!");
            }

            await ctx.StateMachine.MoveTo<SimulationState>();
        }
    }
}