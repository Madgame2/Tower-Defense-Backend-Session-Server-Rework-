using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Application.Session.Services.Repositories;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Application.Session.Services.ObjectRegister.Handlers
{
    public sealed class HitBoxRegesterHandler : IObjectHandler<HitBoxComponent>
    {
        public Type StoragedType => typeof(HitBoxComponent);

        private readonly HitBoxRepository _repository;

        public HitBoxRegesterHandler(HitBoxRepository repository)
        {
            _repository = repository;
        }

        public void Add(HitBoxComponent newObject)
        {
            _repository.Add(newObject);
        }

        public void Change(HitBoxComponent obj)
        {
            // Пока ничего не требуется.
        }

        public ReadOnlySpan<HitBoxComponent> GetAll()
        {
            return _repository.GetAll();
        }

        public void Remove(HitBoxComponent obj)
        {
            _repository.Remove(obj);
        }
    }
}
