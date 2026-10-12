using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.IndexSearchModule.MetaDatas;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules
{
    internal class IndexSearchCompilerModule : ICompilerModule
    {
        public ExecutionStrategyType StategyType => ExecutionStrategyType.IndexScan;

        public void Compile<T>(ref PhysicalPlanNode<T> node, QueryContext<T> queryContext)
        {
            if (node.State is not IndexMetaData<T> metadata)
            {
                throw new InvalidOperationException(
                    $"Metadata at {this} has wrong type");
            }

            metadata.Executor = metadata.QueryAdapter.CreateExecutor(metadata.Index);
            var operation = node.QueryOperation;

            node.executionDelegate = (object state, QueryContext<T> context, List<T> buffer) =>
            {
                var indexMetadata = (IndexMetaData<T>)state;

                indexMetadata.Executor(indexMetadata.Index, operation, context, buffer);
            };
        }
    }
}
