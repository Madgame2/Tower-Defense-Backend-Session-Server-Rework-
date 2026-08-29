using GameServer.Application.Sessions.Repository;
using GameServer.Application.Sessions.Repository.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.Sessions.Repository
{
    public class SessionRegistry : ISessionRegistry
    {
        public GameSessionSnapshot? Get(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public void Register(GameSessionSnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public void Unregister(Guid sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
