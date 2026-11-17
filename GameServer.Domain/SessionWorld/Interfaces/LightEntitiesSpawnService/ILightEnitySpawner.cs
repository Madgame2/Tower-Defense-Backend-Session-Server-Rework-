using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService
{
    public interface ILightEntitySpawner
    {
        IEntityBuilder CreateEntity();

        void Init(GameRoom gameroom, int prewarm =16);
    }

    public interface ILightEntitySpawner<TOut> : ILightEntitySpawner
    {
        new IEntityBuilder<TOut> CreateEntity();
    }
}
