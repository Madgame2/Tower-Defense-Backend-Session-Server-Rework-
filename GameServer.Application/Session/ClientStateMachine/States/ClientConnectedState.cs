using GameServer.Application.Model;
using GameServer.Application.Session.ClientStateMachine.States.Base;
using GameServer.Application.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.States
{
    public class ClientConnectedState : ClientStateBase<ClientRegisterState>, IInitialClientState
    {
        public override async Task OnEnter(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Client {connection.UserId} on state {this.GetType().Name}");
        }

        public override async Task WaitForTransitionAsync(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            await session.WaitForRoomAsync(cancellationToken);
        }

        public override async Task OnExit(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Client {connection.UserId} leave from state {this.GetType().Name}");

        }
    }
}
