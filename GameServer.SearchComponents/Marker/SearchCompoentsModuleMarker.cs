using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using GameServer.SearchComponents.Adapters.FullScan;
using GameServer.SearchComponents.AdaptersRegistry.FullScan;
using GameServer.SearchComponents.AdaptersRegistry.Index;
using GameServer.SearchComponents.AdaptersRegistry.Index.Model;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.IndexSearchModule;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.MemmoryFilterModule;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Implementation;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModulesRegister.Implementation;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModulesRegister.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.Implemintation;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.Interface;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Algorithms.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Algorithms.GreedyAlgorithm;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.CombineModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Implementation;
using GameServer.SearchComponents.TypesTranslator.Builder.Implementation;
using GameServer.SearchComponents.TypesTranslator.Builder.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.SearchComponents.Marker
{
    public class SearchCompoentsModuleMarker
    {
    }

    public static class LibExtentions
    {
        public static IServiceCollection AddSearchCompoents(this IServiceCollection services)
        {
            services.AddSingleton<IFullScanAdapter, RadiusFullScanAdapter>();

            services.AddSingleton<ITranslatorBuilder, TranslatorBuilder>();

            services.AddSingleton<FullScanAdapterRegistry>();

            services.AddSingleton<IFullScanAdapterRegistry>(
                sp => sp.GetRequiredService<FullScanAdapterRegistry>());

            services.AddSingleton<IPossibilitySoureseModuel, FullScanModule>();
            services.AddSingleton<IPossibilitySoureseModuel, IndexSearchModule>();
            services.AddSingleton<IPossibilitySoureseModuel, MemmoryFilter>();

            services.AddSingleton<QueryAdapterRegistry>();
            services.AddSingleton<IQueryAdapterRegistry>(sp => sp.GetRequiredService<QueryAdapterRegistry>());

            services.AddQueryCapabilityAssembly<QueryAdapterRegistry>();
            services.AddQueryCapabilityAssembly<IQueryAdapterRegistry>();

            services.AddSingleton<IQueryPlanerCanditatesGenerator, CanditatesGenerator>();

            services.AddSingleton<IQueryPlanOptimizer, PlanOptimizer>();
            services.AddSingleton<IOptimizeStrategy, GreedyAlgorithmModule>();

            services.AddSingleton<IPlanCombiner, ExecutionSteps.PlanOptimizer.CombineModules.MemmoryFiletModule.MemmoryFilter>();

            services.AddSingleton<IPlanCompiler, ExecutionPlanCompiler>();
            services.AddSingleton<ICompilersModulesRegester, CompilerModuleRegester>();
            services.AddSingleton<ICompilerModule, IndexSearchCompilerModule>();
            services.AddSingleton<ICompilerModule, FullScanCompilerModule>();

            return services;
        }

        public static IServiceCollection AddQueryCapabilityAssembly<T>( this IServiceCollection services)
        {
            services.AddSingleton(
                new QueryCapabilityAssembly(typeof(T).Assembly));

            return services;
        }
    }
}
