using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Interfaces
{
    public interface ISyncService
    {
        Task SyncSessionUsersAsync(Guid sessionId, CancellationToken token = default);
    }
}
