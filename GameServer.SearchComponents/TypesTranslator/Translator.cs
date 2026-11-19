using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.DataBase.Models;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Delegates;
using GameServer.SearchComponents.TypesTranslator.Delegates;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GameServer.SearchComponents.TypesTranslator
{
    public abstract class Translator
    {
        public Type InputType { get; protected set; }
        public Type OutputType { get; protected set; }

        public abstract Delegate CreateExecutorUntyped();

        protected Dictionary<Type, List<Type>> _linkedNodes = new();
        protected Dictionary<(Type from, Type to), List<Delegate>> _transitions = new();
        protected Dictionary<Type, List<TransitionModel>> _graph = new();
    }

    public class Translator<TIn, TOut> : Translator
    {
        public Translator(TransitionModel[][] transitions)
        {
            InputType = typeof(TIn);
            OutputType = typeof(TOut);

            foreach (var transition in transitions)
            {
                foreach (var model in transition)
                    RegTrasition(model);
            }
        }

        private void RegTrasition(TransitionModel transition)
        {
            var from = transition.From;
            var to = transition.To;
            var executor = transition.transitionDelegate;

            if (!_linkedNodes.TryGetValue(from, out var listOfTo))
            {
                listOfTo = new();
                _linkedNodes[from] = listOfTo;
            }
            listOfTo.Add(to);

            var key = (from, to);
            if (!_transitions.TryGetValue(key, out var listOfExecutor))
            {
                listOfExecutor = new();
                _transitions[key] = listOfExecutor;
            }
            listOfExecutor.Add(executor);
        }

        public TranslatorDelegate<TIn, TOut> CreateExecutor()
        {
            var input = Expression.Parameter(
                typeof(TIn).MakeByRefType(), "input");

            var output = Expression.Parameter(
                typeof(TOut).MakeByRefType(), "output");

            var inputLocal = Expression.Variable(
                typeof(TIn), "inputLocal");

            var visited = new HashSet<Type>();

            var body = BuildNode(
                typeof(TIn),
                typeof(TOut),
                inputLocal,
                output,
                visited);

            var block = Expression.Block(
                    new[] { inputLocal },

                    Expression.Assign(inputLocal, input),

                    Expression.Assign(
                        output,
                        Expression.Default(typeof(TOut))),

                    body
                );

            return Expression.Lambda<TranslatorDelegate<TIn, TOut>>(
                            block,
                            input,
                            output
                        ).Compile();
        }

        private Expression BuildNode(
            Type currentType,
            Type targetType,
            Expression currentValue,
            ParameterExpression output,
            HashSet<Type> visited)
        {
            if (currentType == targetType)
            {
                return Expression.Block(
                    Expression.Assign(output, currentValue),
                    Expression.Constant(true)
                );
            }

            if (!visited.Add(currentType))
                return Expression.Constant(false);

            if (!_graph.TryGetValue(currentType, out var edges))
            {
                visited.Remove(currentType);
                return Expression.Constant(false);
            }

            var alternatives = new List<Expression>();

            foreach (var edge in edges)
            {
                var nextType = edge.To;

                if (visited.Contains(nextType))
                    continue;

                var nextValue = Expression.Variable(
                            nextType,
                            $"value_{nextType.Name}");

                var transitionCall = BuildTransition(
                    edge.transitionDelegate,
                    currentType,
                    nextType,
                    currentValue,
                    nextValue,
                    out bool isTry);

                var nextNode = BuildNode(
                            nextType,
                            targetType,
                            nextValue,
                            output,
                            visited);

                Expression branch;

                if (isTry)
                {
                    branch = Expression.AndAlso(
                        transitionCall,
                        nextNode);
                }
                else
                {
                    branch = Expression.Block(
                        transitionCall,
                        nextNode);
                }

                alternatives.Add(
                    Expression.Block(
                        new[] { nextValue },
                        branch
                        )
                    );
            }
            visited.Remove(currentType);

            if (alternatives.Count == 0)
                return Expression.Constant(false);

            if (alternatives.Count == 1)
                return alternatives[0];

            Expression result = alternatives[^1];

            for (int i = alternatives.Count - 2; i >= 0; i--)
            {
                result = Expression.OrElse(
                    alternatives[i],
                    result);
            }

            return result;
        }

        private static Expression BuildTransition(
            Delegate executor,
            Type from,
            Type to,
            Expression input,
            ParameterExpression output,
            out bool isTry)
        {
            var delegateType = executor.GetType();
            var invoke = delegateType.GetMethod("Invoke");

            if (invoke == null)
                throw new InvalidOperationException(
                    "Invalid transition delegate.");

            var instance = Expression.Constant(executor, delegateType);

            if (delegateType.IsGenericType &&
                delegateType.GetGenericTypeDefinition() ==
                typeof(TransitionGetDelegate<,>))
            {
                isTry = true;

                ValidateTypes(invoke, from, to, true);

                return Expression.Invoke(instance, input, output);
            }

            if (delegateType.IsGenericType &&
                delegateType.GetGenericTypeDefinition() ==
                typeof(Func<,>))
            {
                isTry = false;

                ValidateTypes(invoke, from, to, false);

                return Expression.Assign(
                    output,
                    Expression.Invoke(instance, input));
            }

            throw new InvalidOperationException(
                $"Unsupported transition delegate: {delegateType}");
        }

        private static void ValidateTypes(
            System.Reflection.MethodInfo invoke,
            Type from,
            Type to,
            bool tryDelegate)
        {
            var parameters = invoke.GetParameters();

            if (tryDelegate)
            {
                if (parameters.Length != 2 ||
                    parameters[0].ParameterType != from.MakeByRefType() ||
                    parameters[1].ParameterType != to.MakeByRefType() ||
                    invoke.ReturnType != typeof(bool))
                {
                    throw new InvalidOperationException(
                        "Invalid TransitionGetDelegate signature.");
                }
            }
            else
            {
                if (parameters.Length != 1 ||
                    parameters[0].ParameterType != from ||
                    invoke.ReturnType != to)
                {
                    throw new InvalidOperationException(
                        "Invalid Func signature.");
                }
            }
        }

        public override Delegate CreateExecutorUntyped()
        {
            return CreateExecutor();
        }
    }
}
