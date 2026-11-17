using GameServer.Application.Session.Services.QueryServices;
using GameServer.Application.Session.Services.QueryServices.Engine;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Implementation;
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

            return services;
        }
    }
}
