using GameServer.Application.Messaging;
using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.Interface;
using System.Buffers;


namespace GameServer.Application.Session.Services.QueryServices.Engine
{
    public class QueryPlaner : IQueryPlanner
    {
        private readonly IQueryPlanerCanditatesGenerator _queryPlanerCanditatesGenerator;
        private readonly IQueryPlanOptimizer _planOptimizer;
        private readonly IPlanCompiler _executionPlanCompiler;

        public QueryPlaner(IQueryPlanerCanditatesGenerator queryPlanerCanditatesGenerator,
            IQueryPlanOptimizer queryPlanOptimizer,
            IPlanCompiler executionPlanCompiler)
        {
            _queryPlanerCanditatesGenerator = queryPlanerCanditatesGenerator;
            _planOptimizer = queryPlanOptimizer;
            _executionPlanCompiler = executionPlanCompiler;
        }

        public QueryExecutePlan<T> Build<T>(WorldQuery<T> query, QueryContext<T> queryContext)
        {
            var candidatesMatrix = _queryPlanerCanditatesGenerator.Generate<T>(query.Operations, queryContext);
            var planTree = _planOptimizer.Optimize<T>(candidatesMatrix, queryContext);
            var compiledPlanTree = _executionPlanCompiler.Compile(planTree, queryContext);
            var resultPlan = FlatTree<T>(compiledPlanTree);

            return resultPlan;
        }

        private QueryExecutePlan<T> FlatTree<T>(IPlanNode<T> planTree)
        {
            var flatList = new List<IPlanNode<T>>();
            var nodesStack = new Stack<(IPlanNode<T> Node, bool Visited)>();

            nodesStack.Push((planTree, false));

            while (nodesStack.TryPop(out var item))
            {
                var currentNode = item.Node;

                if (item.Visited)
                {
                    flatList.Add(currentNode);
                    continue;
                }


                nodesStack.Push((currentNode, true));

                if (currentNode.Children == null)
                    continue;


                foreach (var childNode in currentNode.Children)
                {
                    nodesStack.Push((childNode, false));
                }
            }

            var executionPlan = new QueryExecutePlan<T>(flatList.Count);

            foreach (var node in flatList)
            {
                var queryExecutionStep = new QueryExecutionStep<T>(node.MetaData, node.Executor);

                executionPlan.Add(queryExecutionStep);
            }

            return executionPlan;
        }
    }
}
