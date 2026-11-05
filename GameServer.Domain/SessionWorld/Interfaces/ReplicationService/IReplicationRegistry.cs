using GameServer.Domain.SessionWorld.Interfaces.ReplicationService;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface IReplicationRegistry
    {
        void RegisterModule(IReplicationModule module);

        void CollectChanges(List<ServerEvent> result);
    }
}
