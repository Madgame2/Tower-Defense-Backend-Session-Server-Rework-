using GameServer.Contracts.MessageFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Interfaces
{
    public interface IMessageSerializer
    {
        (byte[] Buffer, int Length) SerializePooled<T>(T message, MessageFormat format);

        byte[] SerializeToArray<T>(T message, MessageFormat format);
    }
}
