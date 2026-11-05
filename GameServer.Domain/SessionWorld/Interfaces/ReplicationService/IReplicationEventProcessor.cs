using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService
{
    public interface IReplicationEventProcessor
    {
        void Process(
            IReadOnlyList<ReplicationLogEntry> entries,
            List<ServerEvent> result
        );
    }
}
