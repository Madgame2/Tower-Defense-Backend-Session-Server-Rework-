using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta
{
    public enum EventType:byte
    {
        SpawnEvent,
        DespawnEvent,
        GamePlayEvent
    }
}
