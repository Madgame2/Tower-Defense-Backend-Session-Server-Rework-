using GameServer.Contracts.Enums;
using GameServer.Contracts.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.ServerMessages
{
    public struct UdpTokenMessage : IServerMessage
    {
        public ServerAction Action => ServerAction.UPD_TOKEN;

        public uint UpdToken { get;}

        public UdpTokenMessage(uint udpToken)
        {
            UpdToken = udpToken;
        }
    }
}
