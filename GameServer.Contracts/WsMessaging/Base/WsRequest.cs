using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.WsMessaging.Base
{
    public class WsRequest : WsMessage
    {
        public Guid RequestId { get; set; }
    }
}
