using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.ReplicationService.Registry
{
    public class ReplicationRegistry : IReplicationRegistry, IDisposable
    {
        private readonly List<IDisposable> _subscriptions = new();
        private readonly ConcurrentQueue<ServerEvent> _events = new();

        public void RegisterModule(IReplicationModule module)
        {
            void Handler(ServerEvent evt)
            {
                _events.Enqueue(evt);
            }

            module.OnReplication += Handler;

            _subscriptions.Add(new Subscription(() =>
            {
                module.OnReplication -= Handler;
            }));
        }

        public void CollectChanges(List<ServerEvent> result)
        {
            result.Clear();

            while (_events.TryDequeue(out var evt))
                result.Add(evt);
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();

            while (_events.TryDequeue(out _)) { }
        }

        private sealed class Subscription : IDisposable
        {
            private readonly Action _unsubscribe;

            public Subscription(Action unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }

            public void Dispose() => _unsubscribe();
        }
    }
}
