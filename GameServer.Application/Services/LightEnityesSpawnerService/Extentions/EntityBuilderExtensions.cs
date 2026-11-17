using EntityLib.Infrastructure;
using EntityLib.Models;
using GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService;
using GameServer.Application.Services.LightEnityesSpawnerService.EnityBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.LightEnityesSpawnerService.Extentions
{
    public static class EntityBuilderExtensions
    {
        public static IEntityBuilder<Entity> With<T>(this IEntityBuilder<Entity> builder, in T component)
            where T : struct, IComponent
        {
            ((Builder)builder).With(component);
            return builder;
        }

        public static IEntityBuilder<Entity> With<T>(this IEntityBuilder<Entity> builder)
            where T : struct, IComponent
        {
            ((Builder)builder).With<T>();
            return builder;
        }
    }
}
