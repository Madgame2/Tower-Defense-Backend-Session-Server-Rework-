using GameServer.Application.Model;
using GameServer.Application.Session.ClientStateMachine.States.Base;
using GameServer.Application.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.States
{
    public class ClientReadyState : ClientStateBase
    {
        public override Type NextState => null;

        public override Task WaitForTransitionAsync(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            return Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }
}
