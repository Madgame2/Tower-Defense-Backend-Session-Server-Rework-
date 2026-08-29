using GameServer.Contracts.UDP.Enums;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Contracts.UDP.Contract
{
    public interface IUdpPacket
    {
        public uint UdpToken {  get; }
        PacketType Type { get; }

        void Serialize(BinaryWriter writer);
    }
}
