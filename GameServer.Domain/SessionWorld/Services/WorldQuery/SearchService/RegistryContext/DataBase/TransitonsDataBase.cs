using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.DataBase
{
    public class TransitonsDataBase : ITransitionsDataBase
    {
        private readonly Dictionary<(Type form, Type to), Delegate> _executions = new();
        private readonly Dictionary<Type, List<Type>> _links = new();

        public void Register(ITranslationRule rule)
        {
            if (!_links.TryGetValue(rule.InputType, out var outputs))
            {
                outputs = new List<Type>();
                _links[rule.InputType] = outputs;
            }

            outputs.Add(rule.OutputType);
            _executions.Add((rule.InputType, rule.OutputType), rule.executor);
        }

        public List<List<Type>> GetAllPaths(Type from, Type to, int maxDepth = int.MaxValue)
        {
            var result = new List<List<Type>>();
            var current = new List<Type> { from };
            var onPath = new HashSet<Type> { from };

            Dfs(from, to, current, onPath, result, maxDepth);

            return result;
        }
        private void Dfs(
    Type node,
    Type target,
    List<Type> current,
    HashSet<Type> onPath,
    List<List<Type>> result,
    int maxDepth)
        {
            if (node == target)
            {
                result.Add(new List<Type>(current)); // копия, т.к. current потом меняется
                return;
            }

            if (current.Count > maxDepth)
                return;

            if (!_links.TryGetValue(node, out var neighbours))
                return;

            foreach (var next in neighbours)
            {
                if (!onPath.Add(next)) // уже в текущем пути -> цикл
                    continue;

                current.Add(next);

                Dfs(next, target, current, onPath, result, maxDepth);

                current.RemoveAt(current.Count - 1); // откат
                onPath.Remove(next);
            }
        }

        public bool TryGetExecutionChain(Type from, Type to, out List<Delegate> chain)
        {
            chain = null;

            if (!TryGetPath(from, to, out var path))
                return false;

            chain = new List<Delegate>(path.Count - 1);
            for (int i = 0; i < path.Count - 1; i++)
                chain.Add(_executions[(path[i], path[i + 1])]);

            return true;
        }

        public bool TryGetPath(Type from, Type to, out List<Type> path)
        {
            path = null;

            if (from == to)
            {
                path = new List<Type> { from };
                return true;
            }

            var queue = new Queue<Type>();
            var cameFrom = new Dictionary<Type, Type>(); // узел -> откуда пришли

            queue.Enqueue(from);
            cameFrom[from] = null;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!_links.TryGetValue(current, out var neighbours))
                    continue;

                foreach (var next in neighbours)
                {
                    if (cameFrom.ContainsKey(next))
                        continue; // уже были

                    cameFrom[next] = current;

                    if (next == to)
                    {
                        path = Restore(cameFrom, to);
                        return true;
                    }

                    queue.Enqueue(next);
                }
            }

            return false; // пути нет
        }

        private static List<Type> Restore(Dictionary<Type, Type> cameFrom, Type to)
        {
            var path = new List<Type>();
            for (var node = to; node != null; node = cameFrom[node])
                path.Add(node);

            path.Reverse();
            return path;
        }
    }
}
