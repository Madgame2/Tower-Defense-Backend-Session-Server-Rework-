using GameServer.Application.Model;
using GameServer.Application.Session.ClientStateMachine.States.Base;
using GameServer.Application.Sessions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.ClientStateMachine.States
{
    public class ClientRegisterState : ClientStateBase<ClientSyncState>
    {
        private readonly ILogger<ClientRegisterState> _logger;

        public ClientRegisterState(ILogger<ClientRegisterState> logger)
        {
            _logger = logger;
        }

        public override async Task OnEnter(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Client {connection.UserId} on state {this.GetType().Name}");
        }

        public override async Task WaitForTransitionAsync(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            try
            {
                await session.CurrentRoom.worldQueryService.RegisterPlayerAsync(connection.UserId, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public override async Task OnExit(ClientConnection connection, GameSession session, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Client {connection.UserId} leave from state {this.GetType().Name}");
        }
    }
}
