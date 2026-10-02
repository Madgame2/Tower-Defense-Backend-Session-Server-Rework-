using GameServer.Application.Session.Services.QueryServices.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameServer.Application.Session.Services.QueryServices
{
    public class QueryCapabilityRegistry : IQueryCapabilityRegistry
    {
        private readonly Dictionary<(Type From, Type To), IQueryAdapter> _adapters = new();

        public QueryCapabilityRegistry(IEnumerable<QueryCapabilityAssembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                RegisterAssembly(assembly.Assembly);
            }
        }

        private void RegisterAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!TryGetAdapterInfo(type, out var from, out var to))
                    continue;

                var adapter = (IQueryAdapter)Activator.CreateInstance(type)!;

                if (!_adapters.TryAdd((from, to), adapter))
                {
                    throw new InvalidOperationException(
                        $"Query adapter for '{from.Name} -> {to.Name}' " +
                        $"is already registered. " +
                        $"Duplicate adapter: {type.FullName}");
                }
            }
        }

        private static bool TryGetAdapterInfo(
            Type type,
            out Type from,
            out Type to)
        {
            from = null!;
            to = null!;

            if (!type.IsClass || type.IsAbstract)
                return false;

            var baseType = type;

            while (baseType != null)
            {
                if (baseType.IsGenericType &&
                    baseType.GetGenericTypeDefinition() == typeof(QueryAdapter<,>))
                {
                    var arguments = baseType.GetGenericArguments();

                    from = arguments[0];
                    to = arguments[1];

                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
