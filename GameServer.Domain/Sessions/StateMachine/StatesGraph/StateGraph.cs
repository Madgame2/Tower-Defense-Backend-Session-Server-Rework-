using GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes;
using GameServer.Domain.Sessions.StateMachine.StatesGraph.Node;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameServer.Domain.Sessions.StateMachine.StatesGraph
{
    public class StateGraph
    {
        private Dictionary<Type, GraphNode> _nodes = new();
        private readonly List<Type> _roots = new();

        public Type RootState
        {
            get
            {
                if (_roots.Count != 1)
                    throw new InvalidOperationException(
                        "Root state is not uniquely defined.");

                return _roots[0];
            }
        }

        public bool CanTransit(Type from, Type to)
        {
            if (!_nodes.TryGetValue(from, out var node))
                return false;

            return node.canTransitTo.Contains(to);
        }
        public void InitGraph(Assembly assembly)
        {
            var states = assembly.GetTypes()
                .Where(t =>
                    typeof(BaseState).IsAssignableFrom(t) &&
                    !t.IsAbstract);

            foreach (var stateType in states)
            {
                var node = new GraphNode(stateType);

                _nodes.Add(stateType, node);

                if (Attribute.IsDefined(
                        stateType,
                        typeof(RootStateAttribute)))
                {
                    _roots.Add(stateType);
                }
            }

            BuildTransitions(states);

            ValidateGraph();
        }
        private void BuildTransitions(IEnumerable<Type> states)
        {
            foreach (var state in states)
            {
                var attrs =
                    state.GetCustomAttributes<TransitionAttribute>();

                foreach (var attr in attrs)
                {
                    _nodes[state]
                        .canTransitTo
                        .Add(attr.TargetState);
                }
            }
        }
        private void ValidateGraph()
        {
            if (_roots.Count == 0)
                throw new InvalidOperationException(
                    "State graph has no root state.");

            if (_roots.Count > 1)
                throw new InvalidOperationException(
                    $"State graph has multiple roots: " +
                    string.Join(", ", _roots));

            ValidateTransitions();
            ValidateDuplicateTransitions();
        }
        private void ValidateDuplicateTransitions()
        {
            foreach (var node in _nodes.Values)
            {
                var duplicates = node.canTransitTo
                    .GroupBy(x => x)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToList();

                if (duplicates.Count == 0)
                    continue;

                throw new InvalidOperationException(
                    $"State '{node.Node.Name}' " +
                    $"contains duplicate transitions: " +
                    string.Join(
                        ", ",
                        duplicates.Select(x => x.Name))
                );
            }
        }
        private void ValidateTransitions()
        {
            foreach (var node in _nodes.Values)
            {
                foreach (var target in node.canTransitTo)
                {
                    if (!_nodes.ContainsKey(target))
                    {
                        throw new InvalidOperationException(
                            $"{node.Node.Name} " +
                            $"transitions to unknown state " +
                            $"{target.Name}");
                    }
                }
            }
        }
    }
}

