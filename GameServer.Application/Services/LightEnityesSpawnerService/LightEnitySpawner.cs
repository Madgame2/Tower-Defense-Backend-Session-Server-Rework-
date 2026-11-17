using EntityLib.GameRoomExtentions;
using EntityLib.Models;
using GameServer.Application.Services.LightEnityesSpawnerService.EnityBuilder;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.LightEnityesSpawner
{
    public class LightEnitySpawner : ILightEntitySpawner<Entity>
    {
        private readonly Stack<Builder> _pool = new Stack<Builder>();
        private GameRoom _linkedRoom;

        public void Init(GameRoom gameRoom, int prewarm = 16)
        {
            _linkedRoom = gameRoom;

            for (int i = 0; i < prewarm; i++)
                _pool.Push(new Builder(this));
        }

        public IEntityBuilder<Entity> CreateEntity()
        {
            var builder = _pool.Count > 0 ? _pool.Pop() : new Builder(this);
            builder.Begin(_linkedRoom);
            return builder;
        }

        IEntityBuilder ILightEntitySpawner.CreateEntity() => CreateEntity();

        internal void Release(Builder builder) => _pool.Push(builder);
    }
}
