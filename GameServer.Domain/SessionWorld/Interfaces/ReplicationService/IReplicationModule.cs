using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ReplicationService
{
    public interface IReplicationModule
    {
        Type ProcessedType { get; }

        event Action<ServerEvent> OnReplication;
    }

    public interface IReplicationModule<T> : IReplicationModule
    {
        Type ProcessedType => typeof(T);
    }
}
