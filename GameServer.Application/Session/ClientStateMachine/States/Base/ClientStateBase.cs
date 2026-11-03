using GameServer.Application.Model;
using GameServer.Application.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.States.Base
{
    public abstract class ClientStateBase
    {
        public abstract Type NextState { get; }

        public virtual Task OnEnter(
            ClientConnection connection,
            GameSession session,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public abstract Task WaitForTransitionAsync(
            ClientConnection connection,
            GameSession session,
            CancellationToken cancellationToken);

        public virtual Task OnExit(
            ClientConnection connection,
            GameSession session,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class ClientStateBase<TNextState> : ClientStateBase where TNextState : ClientStateBase

    {
        public override Type NextState => typeof(TNextState); 
    }
}
