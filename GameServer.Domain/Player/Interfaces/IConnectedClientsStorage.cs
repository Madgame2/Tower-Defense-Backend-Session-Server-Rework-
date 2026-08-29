using GameServer.Application.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Interfaces
{
    public interface IConnectedClientsStorage
    {
        void Add(ClientConnection client);

        Task Remove(string UserId);

        bool TryGet(string UserId, out ClientConnection? client);
        bool tryGetByUdpToken(uint udpToken, out ClientConnection? client);

        IReadOnlyCollection<ClientConnection> GetAll();
    }
}
