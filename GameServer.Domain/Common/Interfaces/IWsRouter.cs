using GameServer.Application.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Interfaces
{
    public interface IWsRouter
    {
        Task HandleAsync(WsMessageContext ctx);

        void On(string eventName, Func<WsMessageContext, Task> handler);

        public void OffAllByEventName(string eventName);
        public void Off(Func<WsMessageContext, Task> handler);
        public void Off(string action, Func<WsMessageContext, Task> handler);

        Task WaitBeforeAsync(string  eventName);
        Task WaitBeforeAsync(string eventName, CancellationToken ct);
    }
}
