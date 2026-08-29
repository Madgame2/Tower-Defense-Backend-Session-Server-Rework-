using GameServer.Domain.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Sessions.Repository
{
    public interface IGameSessionRepository
    {
        GameSession[] GetAll();
        GameSession? Get(Guid id);
        void Save(GameSession session);
        void Remove(Guid id);
    }
}
