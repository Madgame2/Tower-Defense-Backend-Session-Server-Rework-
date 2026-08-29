using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    public struct DecorationRulesMessage : IServerMessage
    {
        public ServerAction Action => ServerAction.METADATA_DECORATION_RULES;

        public byte[] XmlPayload { get; set; }

        public DecorationRulesMessage(byte[] payloadxml)
        {
            this.XmlPayload = payloadxml;
        }
    }
}
