using EntityLib.GameRoomExtentions;
using EntityLib.Infrastructure;
using EntityLib.Models;
using GameServer.Application.Services.LightEnityesSpawner;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService;
using GameServer.СomponentsForLightweightEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.LightEnityesSpawnerService.EnityBuilder
{
    public class Builder : IEntityBuilder<Entity>
    {
        private readonly LightEnitySpawner _owner;
        private GameRoom _room;
        private Entity _entity;
        private bool _active;

        internal Builder(LightEnitySpawner owner)
        {
            _owner = owner;
        }

        // вызывается спавнером при выдаче билдера из пула
        internal void Begin(GameRoom room)
        {
            _room = room;
            _entity = room.CreateEntity();
            _room.GetStash<IDComponent>().Set(_entity);
            _active = true;
        }

        public Builder With<T>(in T component) where T : struct, IComponent
        {
            EnsureActive();
            _room.GetStash<T>().Set(_entity, component);
            return this;
        }

        public Builder With<T>() where T : struct, IComponent
        {
            EnsureActive();
            _room.GetStash<T>().Set(_entity);
            return this;
        }

        public Entity Build()
        {
            EnsureActive();

            var entity = _entity;

            _active = false;
            _room = null;
            _entity = default;

            _owner.Release(this); // возвращаем себя в пул
            return entity;
        }

        private void EnsureActive()
        {
            if (!_active)
                throw new InvalidOperationException(
                    "Builder уже завершён (Build вызван). Получите новый через CreateEntity().");
        }
    }
}
