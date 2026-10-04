using GameServer.Application.Session.Services.QueryServices.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GameServer.Application.Session.Services.QueryServices
{
    public class QueryAdapterRegistry : IQueryAdapterRegistry
    {
        private readonly Dictionary<(Type From, Type To), IQueryAdapter> _adapters = new();
        private readonly Dictionary<Type, List<Type>> _capabilities = new();

        public QueryAdapterRegistry(IEnumerable<QueryCapabilityAssembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                RegisterAssembly(assembly.Assembly);
            }
        }

        public ReadOnlySpan<Type> GetCapabilities<T>(IQueryOperation operation)
        {
            var oeprationType = typeof(T);

            return CollectionsMarshal.AsSpan(_capabilities[oeprationType]);
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

                if(!_capabilities.TryGetValue(from, out var outList)){
                    _capabilities[from] = new List<Type>();
                }
                _capabilities[from].Add(to);
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
