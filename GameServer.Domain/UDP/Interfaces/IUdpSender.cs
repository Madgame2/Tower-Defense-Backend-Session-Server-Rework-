using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GameServer.Domain.UDP.Interfaces
{
    public interface IUdpSender
    {
        ValueTask SendAsync(EndPoint endPoint, ReadOnlyMemory<byte> payload);
    }
}
