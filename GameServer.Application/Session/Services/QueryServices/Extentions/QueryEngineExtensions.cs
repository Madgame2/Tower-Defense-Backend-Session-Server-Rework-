using GameServer.Application.Session.Services.QueryServices;
using GameServer.Application.Session.Services.QueryServices.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameServer.Application.Session.Services.QueryServices.SearchService.Engine.Extentions
{
    public static class QueryEngineExtensions
    {
        public static IServiceCollection AddQueryEngine(this IServiceCollection services)
        {
            services.AddSingleton<IQueryPlanner, QueryPlaner>();
            services.AddSingleton<IQueryExecutor, QueryExecuteEngine>();


            services.AddSingleton<QueryCapabilityRegistry>();
            services.AddSingleton<IQueryCapabilityRegistry>( sp => sp.GetRequiredService<QueryCapabilityRegistry>());

            services.AddQueryCapabilityAssembly<QueryCapabilityRegistry>();
            services.AddQueryCapabilityAssembly<IQueryCapabilityRegistry>();

            return services;
        }

        public static IServiceCollection AddQueryCapabilityAssembly<T>(
            this IServiceCollection services)
        {
            services.AddSingleton(
                new QueryCapabilityAssembly(typeof(T).Assembly));

            return services;
        }
    }
}
