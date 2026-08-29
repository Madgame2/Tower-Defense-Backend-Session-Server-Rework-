using GameServer.Application.Sessions.Repository.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Sessions.Repository
{
    public interface ISessionRegistry
    {
        void Register(GameSessionSnapshot snapshot);
        void Unregister(Guid sessionId);
        GameSessionSnapshot? Get(Guid sessionId);
    }
}
