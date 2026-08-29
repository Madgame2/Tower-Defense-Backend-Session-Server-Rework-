using GameServer.Application.Sessions.Repository.models;
using GameServer.Domain.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Sessions
{
    public interface IGameSessionService
    {
        Task<Guid> CreateSession();
        GameSession? Get(Guid sessionId);
        Task<AttachResult> Attach(string playerId, Guid sessionId);

        Guid[] GetAllIds();
    }
}
