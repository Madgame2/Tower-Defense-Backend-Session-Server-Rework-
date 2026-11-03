using GameServer.Application.Session.Services.GameObjectSpawner.Modules.Base;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.GameObjectSpawner
{
    public class GameObjectSpawner : IGameObjectSpawner
    {
        private readonly IServiceProvider _serviceProvider;


        public GameObjectSpawner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Spawn<T>() where T : GameObject
        {
            return Spawn<T>(Array.Empty<object>());
        }

        public T Spawn<T>(params object[] parameters) where T : GameObject
        {
            var gameObject = ActivatorUtilities.CreateInstance<T>(_serviceProvider,parameters);

            gameObject.AddComponent(new TransformComponent());

            var modules = _serviceProvider.GetServices<IGameObjectSpawnModule<T>>();

            foreach (var module in modules)
                module.Configure(gameObject);

            return gameObject;
        }
    }
}
