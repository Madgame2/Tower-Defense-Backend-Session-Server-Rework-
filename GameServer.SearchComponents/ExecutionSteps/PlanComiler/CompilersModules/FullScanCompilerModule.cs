using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule.MetaDatas;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules
{
    internal class FullScanCompilerModule : ICompilerModule
    {
        public ExecutionStrategyType StategyType => ExecutionStrategyType.FullScan;

        public void Compile<T>(
            ref PhysicalPlanNode<T> node,
            QueryContext<T> queryContext)
        {
            if (node.State is not FullScanMetaData<T> metadata)
            {
                throw new InvalidOperationException(
                    $"Metadata at {this} has wrong type");
            }

            metadata.Executor = metadata.Adapter.CreateExecutor<T>();

            var operation = node.QueryOperation;

            node.executionDelegate = (object state, QueryContext<T> context, List<T> buffer) =>
            {
                var fullScanMetadata = (FullScanMetaData<T>)state;

                ReadOnlySpan<T> source = context.ObjectsRegistry.GetAll<T>();

                fullScanMetadata.Executor(
                    operation,
                    context,
                    source,
                    buffer);
            };
        }
    }
}
