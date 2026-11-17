using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService
{
    public interface IEntityBuilder { }

    public interface IEntityBuilder<out T> : IEntityBuilder
    {
        T Build();
    }
}
