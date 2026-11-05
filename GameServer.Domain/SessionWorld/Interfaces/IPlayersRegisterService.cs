using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface IPlayersRegisterService
    {
        Task<Player.Player> RegisterPlayerAsync(string userID, CancellationToken cancellationToken);
    }
}
