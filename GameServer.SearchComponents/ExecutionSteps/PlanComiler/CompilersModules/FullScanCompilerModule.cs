using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.FullScanModule.MetaDatas;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using GameServer.SearchComponents.TypesTranslator.Delegates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules
{
    internal class FullScanCompilerModule : ICompilerModule
    {
        public ExecutionStrategyType StategyType => ExecutionStrategyType.FullScan;

        private static readonly MethodInfo CombinedExecutorMethod =
    typeof(FullScanCompilerModule).GetMethod(
        nameof(CreateCombinedExecutor),
        BindingFlags.Static | BindingFlags.NonPublic);

        public void Compile<T>(
            ref PhysicalPlanNode<T> node,
            QueryContext<T> queryContext)
        {
            if (node.State is not FullScanMetaData<T> metadata)
            {
                throw new InvalidOperationException(
                    $"Metadata at {this} has wrong type");
            }

            Type outputType = metadata.Adapter.RequriredType;

            metadata.Executor = BuildExecutor<T>(metadata);

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

        public static FullScanExecutorDelegate<T> BuildExecutor<T>(
    FullScanMetaData<T> metadata)
        {
            var translator = metadata.Translator;
            var adapter = metadata.Adapter;

            MethodInfo closedMethod = CombinedExecutorMethod.MakeGenericMethod(
                translator.InputType,
                translator.OutputType);

            Delegate translationExecution =
                translator.CreateExecutorUntyped();

            Delegate adapterExecution =
                adapter.CreateExecutorUntyped<T>();

            return (FullScanExecutorDelegate<T>)closedMethod.Invoke(
                null,
                new object[]
                {
            translationExecution,
            adapterExecution
                });
        }

        private static FullScanExecutorDelegate<T> CreateCombinedExecutor<T, TOut>(
            TranslatorDelegate<T, TOut> translator,
            ScanAdapterDelegate<T, TOut> adapter)
        {
            return (operation, context, source, outBuffer) =>
            {
                for (int i = 0; i < source.Length; i++)
                {
                    TOut translated;

                    translator(in source[i], out translated);

                    bool shouldAdd = adapter(
                        translated,
                        operation,
                        context);

                    if (shouldAdd)
                        outBuffer.Add(source[i]);
                }
            };
        }
    }
}
