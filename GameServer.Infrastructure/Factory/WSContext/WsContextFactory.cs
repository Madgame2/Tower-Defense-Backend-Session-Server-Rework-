using GameServer.Application.Interfaces;
using GameServer.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace GameServer.Infrastructure.Factory.WSContext
{
    public class WsContextFactory : IWsContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WsContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public WsContext Create(WebSocket socket, string playerId, Guid sessionId)
        {
            var router = _serviceProvider.GetRequiredService<IWsRouter>();

            return new WsContext
            {
                Socket = socket,
                PlayerId = playerId,
                SessionId = sessionId,
                Router = router
            };
        }
    }
}
