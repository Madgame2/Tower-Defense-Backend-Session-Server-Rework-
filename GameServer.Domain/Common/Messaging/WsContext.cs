using GameServer.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace GameServer.Application.Messaging
{
    public class WsContext
    {
        public WebSocket Socket { get; init; }
        
        public string PlayerId {  get; init; }
        public Guid SessionId {  get; init; }

        public IWsRouter Router { get; init; }
    }
}
