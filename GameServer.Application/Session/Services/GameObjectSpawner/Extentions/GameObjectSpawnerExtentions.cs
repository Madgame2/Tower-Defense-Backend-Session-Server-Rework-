using GameServer.Application.Session.Services.GameObjectSpawner.Modules;
using GameServer.Application.Session.Services.GameObjectSpawner.Modules.Base;
using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.GameObjectSpawner.Extentions
{
    public static class GameObjectSpawnerExtentions
    {

        public static void AddGameObjectSpawnerService(this IServiceCollection serviceProvider)
        {
            serviceProvider.AddSingleton<IGameObjectSpawnModule<Player>, PlayerGameObjectSpawnModule>();


            serviceProvider.AddTransient<IGameObjectSpawner, GameObjectSpawner>();
        }
    }
}
