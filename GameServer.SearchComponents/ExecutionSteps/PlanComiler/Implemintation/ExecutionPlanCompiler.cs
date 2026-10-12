using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModulesRegister.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.Interface;
using GameServer.SearchComponents.ExecutionSteps.PlanOptimizer.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.Implemintation
{
    public class ExecutionPlanCompiler : IPlanCompiler
    {
        private readonly ICompilersModulesRegester _modulesRegester;


        public ExecutionPlanCompiler(ICompilersModulesRegester compilersModulesRegester)
        {
            _modulesRegester = compilersModulesRegester;
        }

        public IPlanNode<T> Compile<T>(IPlanNode<T> planRoot, QueryContext<T> queryContext)
        {
            if (planRoot is not PhysicalPlanNode<T> root)
            {
                throw new InvalidOperationException(
                    $"Unsupported plan node type: {planRoot.GetType()}.");
            }

            var stack = new Stack<(PhysicalPlanNode<T> Node, bool Visited)>();
            var compiledNodes = new Stack<PhysicalPlanNode<T>>();

            stack.Push((root, false));

            while (stack.TryPop(out var entry))
            {
                var node = entry.Node;

                if (!entry.Visited)
                {
                    stack.Push((node, true));

                    var children = node.ChildNodes;

                    for (int i = children.Length - 1; i >= 0; i--)
                    {
                        stack.Push((children[i], false));
                    }

                    continue;
                }

                var childrenCount = node.ChildNodes.Length;

                if (childrenCount > 0)
                {
                    var compiledChildren = new PhysicalPlanNode<T>[childrenCount];

                    for (int i = childrenCount - 1; i >= 0; i--)
                    {
                        compiledChildren[i] = compiledNodes.Pop();
                    }

                    node.ChildNodes = compiledChildren;
                }

                node = CompileNode(node, queryContext);

                compiledNodes.Push(node);
            }

            return compiledNodes.Pop();
        }

        private PhysicalPlanNode<T> CompileNode<T>(
            PhysicalPlanNode<T> node,
            QueryContext<T> queryContext)
        {
            if (!_modulesRegester.TryResolveModule(
                    node.ExecutionStrategy,
                    out var compilerModule))
            {
                throw new InvalidOperationException(
                    $"Compiler module not found for execution strategy " +
                    $"'{node.ExecutionStrategy}'.");
            }

            compilerModule.Compile(ref node, queryContext);

            return node;
        }
    }
}
