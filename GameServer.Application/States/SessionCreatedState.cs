using GameServer.Domain.Sessions;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.Events.Stateshandler;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Sessions.States
{
    [RootState]
    [Transition(typeof(GenerateWorldState))]
    public class SessionCreatedState : BaseState
    {

        public override void Configure(StateEventHandler handlers)
        {
            base.Configure(handlers);

            handlers.Register<PlayerConnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player join {evt.PlayerId}");
                if (ctx.OnlinePlayersIDs.Count == ctx.ResurvatePlayersIDs.Count) {
                    Console.WriteLine("AllPlayers connected");

                    try
                    {
                        await ctx.StateMachine.MoveTo<GenerateWorldState>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

            handlers.Register<PlayerDisconnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player leave {evt.PlayerId}");
            });
        }
        public override Task OnEnter(GameSession ctx)
        {
            return base.OnEnter(ctx);
        }
    }
}
