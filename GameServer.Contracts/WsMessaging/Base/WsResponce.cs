using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.WsMessaging.Base
{
    public class WsResponce
    {
        public Guid RequestId { get; set; }
        public int StatusCode { get; set; }
        public bool Secsess { get; set; }
        public string Message { get; set; }
        public object Payload { get; set; }
    }
}
