using EntityLib.Models;
using GameServer.Application.Services.LightEnityesSpawner;
using GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.LightEnityesSpawnerService.Extentions
{
    public static class LightEntitySpawnerServiceCollectionExtensions
    {
        public static IServiceCollection AddLightEntitySpawner(this IServiceCollection services)
        {
            services.AddSingleton<LightEnitySpawner>();
            services.AddSingleton<ILightEntitySpawner<Entity>>(sp => sp.GetRequiredService<LightEnitySpawner>());
            services.AddSingleton<ILightEntitySpawner>(sp => sp.GetRequiredService<LightEnitySpawner>());

            return services;
        }
    }
}
