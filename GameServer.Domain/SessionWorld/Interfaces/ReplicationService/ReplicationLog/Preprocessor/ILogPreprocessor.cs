using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Preprocessor
{
    public interface ILogPreprocessor
    {
        int Process(uint playerId, uint lastAcknowledgedTick, ReplicationLog log, out ReadOnlyMemory<ReplicationLogEntry> entriesToSend);
    }
}
