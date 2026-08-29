using GameServer.Application.Sessions;
using GameServer.Application.Sessions.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Sessions.Repository
{
    public class InMemmoryGameSessionRepository : IGameSessionRepository
    {
        private Dictionary<Guid, GameSession> _strorage = new();

        public GameSession? Get(Guid id)
        {
            return _strorage[id];
        }

        public GameSession[] GetAll()
        {
            return _strorage.Values.ToArray();
        }

        public void Remove(Guid id)
        {
            _strorage.Remove(id);
        }

        public void Save(GameSession session)
        {
            _strorage.Add(session.Id, session);
        }
    }
}
