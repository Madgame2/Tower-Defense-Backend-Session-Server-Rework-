using GameServer.Application.Sessions.Repository;
using GameServer.Application.Sessions.Repository.models;
using GameServer.Domain.Sessions;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Domain.Sessions.StateMachine.Factory;
using GameServer.Domain.Sessions.StateMachine.StatesGraph;
using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GameServer.Application.Sessions.imp
{
    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionRepository _sessionRep;
        private readonly ISessionRegistry _sessionRegistry;
        private readonly GameSessionFactory _sessionFactory;

        public GameSessionService(
            IGameSessionRepository sessionRep,
            ISessionRegistry sessionRegistry,
            GameSessionFactory sessionFactory)
        {
            _sessionRep = sessionRep;
            _sessionRegistry = sessionRegistry;
            _sessionFactory = sessionFactory;
        }

        public async Task<AttachResult> Attach(string playerId, Guid sessionId)
        {
            var session = _sessionRep.Get(sessionId);

            if (session == null)
            {
                return new AttachResult
                {
                    Success = false,
                    ErrorMessage = "No such session"
                };
            }

            if (!session.hasPlayerReserwation(playerId))
            {
                return new AttachResult
                {
                    Success = false,
                    ErrorMessage = "Forbidden"
                };
            }

            session.Enqueue(new PlayerConnected(playerId));


            return new AttachResult
            {
                Success = true,
            };
        }



        private void removeSessionCallback(Guid sessionId)
        {
            var session = _sessionRep.Get(sessionId);
            if (session == null) return;

            _sessionRep.Remove(sessionId);

            session._terminateSession -= removeSessionCallback;
        }

        public async Task<Guid> CreateSession()
        {
            var session = _sessionFactory.Create();
            session._terminateSession += removeSessionCallback;

            _sessionRep.Save(session);

            await session.StartSession();

            return session.Id;
        }

        public GameSession? Get(Guid sessionId)
        {
            return _sessionRep.Get(sessionId);
        }

        public Guid[] GetAllIds()
        {
            var all = _sessionRep.GetAll();

            return all.Select(x => x.Id).ToArray();
        }
    }

    public class GameSessionFactory
    {
        private readonly StateGraph _graph;
        private readonly StateFactory _stateFactory;

        public GameSessionFactory(
            StateGraph graph,
            StateFactory stateFactory)
        {
            _graph = graph;
            _stateFactory = stateFactory;
        }

        public GameSession Create()
        {
            var session = new GameSession();

            session.Id = Guid.NewGuid();

            var stateMachine =
                new SessionStateMachine(
                    session,
                    _graph,
                    _stateFactory);

            session.Init(stateMachine);

            return session;
        }
    }
}
