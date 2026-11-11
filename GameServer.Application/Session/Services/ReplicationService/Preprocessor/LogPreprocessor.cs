using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.ReplicationLog.Preprocessor;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.ReplicationService.Preprocessor
{
    public class LogPreprocessor : ILogPreprocessor
    {
        public int Process(
            uint playerId,
            uint lastAcknowledgedTick,
            ReplicationLog log,
            out ReadOnlyMemory<ReplicationLogEntry> entriesToSend)
        {
            var entries = log.Entries.Span;

            int startIndex = 0;

            while (startIndex < entries.Length &&
                   entries[startIndex].Tick <= lastAcknowledgedTick)
            {
                startIndex++;
            }

            entriesToSend = log.Entries.Slice(startIndex);

            return entriesToSend.Length;
        }
    }
}
