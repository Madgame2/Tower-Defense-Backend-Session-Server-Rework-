using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Interfaces
{
    public interface ISyncService
    {
        Task SyncSessionUsersAsync(Guid sessionId, CancellationToken token = default);

        Task SyncPlayerAsync(Guid sessionId, string playerId, CancellationToken token = default);
    }
}
