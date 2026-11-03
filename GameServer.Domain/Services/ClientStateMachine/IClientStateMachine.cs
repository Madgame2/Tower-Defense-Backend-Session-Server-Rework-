using GameServer.Application.Model;
using GameServer.Application.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Services.ClientStateMachine
{
    public interface IClientStateMachine
    {
        void Init(ClientConnection clientConnection, GameSession session);
        void Start(CancellationToken cancellationToken);
        Task StopAsync();
    }
}
