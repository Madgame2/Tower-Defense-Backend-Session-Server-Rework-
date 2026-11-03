using GameServer.Application.Model;
using GameServer.Application.Session.ClientStateMachine.States.Base;
using GameServer.Application.Sessions;
using GameServer.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.States
{
    public class ClientSyncState : ClientStateBase<ClientReadyState>
    {
        private readonly ISyncService _syncService;

        public ClientSyncState(ISyncService syncService)
        {
            _syncService = syncService;
        }

        public override async Task OnEnter(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            await _syncService.SyncPlayerAsync(session.Id, connection.UserId, cancellationToken);

            session.SyncBarrier.Complete(connection.UserId);
        }

        public override async Task WaitForTransitionAsync(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            return;
        }
    }
}
