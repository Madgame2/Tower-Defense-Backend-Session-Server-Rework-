using GameServer.Application.Interfaces;
using GameServer.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.WSRouter
{
    public class WsRouter : IWsRouter, IDisposable
    {
        private Dictionary<string, List<Func<WsMessageContext, Task>>> _handlers = new();
        private readonly Dictionary<string, List<TaskCompletionSource>> _waiters = new();
        private bool _disposed;

        public async Task HandleAsync(WsMessageContext ctx)
        {
            var message = ctx.Message;
            if (message == null) return;

            if (_waiters.TryGetValue(message.Action, out var list) && list.Count > 0)
            {
                lock (_waiters)
                {
                    var waitersToNotify = list.ToList();

                    list.Clear();

                    foreach (var tcs in waitersToNotify)
                    {
                        tcs.TrySetResult();
                    }
                }
            }

            if (!_handlers.ContainsKey(message.Action)) return;

            foreach(var handler in _handlers[message.Action])
            {
                await handler(ctx);
            }
        }

        public void Off(Func<WsMessageContext, Task> handler)
        {
            foreach (var handlers in _handlers.Values)
            {
                handlers.RemoveAll(i=> i==handler);
            }
        }

        public void Off(string action, Func<WsMessageContext, Task> handler)
        {
            if (!_handlers.ContainsKey(action)) return;

            _handlers[action] = _handlers[action].FindAll(i => i != handler);
        }

        public void OffAllByEventName(string eventName)
        {
            if (_handlers.ContainsKey(eventName))
            {
                _handlers[eventName].Clear();
            }
        }

        public void On(string eventName, Func<WsMessageContext, Task> handler)
        {
            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new();
            }
            _handlers[eventName].Add(handler);
        }

        public async Task WaitBeforeAsync(string eventName)
        {
            var tcs = new TaskCompletionSource();
            lock (_waiters) 
            {
                if (!_waiters.ContainsKey(eventName))
                    _waiters[eventName] = new();
                _waiters[eventName].Add(tcs);
            }
            await tcs.Task;
        }

        public async Task WaitBeforeAsync(string eventName, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            using (ct.Register(() => tcs.TrySetCanceled(ct)))
            {
                lock (_waiters)
                {
                    if (!_waiters.ContainsKey(eventName))
                        _waiters[eventName] = new();
                    _waiters[eventName].Add(tcs);
                }

                try
                {
                    await tcs.Task;
                }
                catch (OperationCanceledException)
                {
                    lock (_waiters)
                    {
                        if (_waiters.TryGetValue(eventName, out var list))
                        {
                            list.Remove(tcs);
                        }
                    }
                    throw;
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_waiters)
            {
                foreach (var waitersList in _waiters.Values)
                {
                    foreach (var tcs in waitersList)
                    {
                        tcs.TrySetCanceled();
                    }
                    waitersList.Clear();
                }
                _waiters.Clear();
            }

            // 2. Очищаем все обработчики
            _handlers.Clear();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
