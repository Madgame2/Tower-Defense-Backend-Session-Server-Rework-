using GameServer.Application.Session.Services.QueryServices.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
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

        private readonly Dictionary<Type, List<IQueryAdapter>> _operationAdapters = new();

        public QueryAdapterRegistry(IEnumerable<QueryCapabilityAssembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    RegisterAssembly(assembly.Assembly);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public ReadOnlySpan<Type> GetCapabilities<T>(IQueryOperation operation)
        {
            var operationType = typeof(T);
            return _capabilities.TryGetValue(operationType, out var list)
                ? CollectionsMarshal.AsSpan(list)
                : ReadOnlySpan<Type>.Empty;
        }

        public int GetCapabilities<T>(
                IQueryOperation operation,
                out ReadOnlySpan<Type> indexQueriesTypes,
                out ReadOnlySpan<IQueryAdapter> adapters)
        {
            var operationType = operation.GetType();

            if (_capabilities.TryGetValue(operationType, out var types) &&
                _operationAdapters.TryGetValue(operationType, out var adapterList))
            {
                indexQueriesTypes = CollectionsMarshal.AsSpan(types);
                adapters = CollectionsMarshal.AsSpan(adapterList);
            }
            else
            {
                indexQueriesTypes = ReadOnlySpan<Type>.Empty;
                adapters = ReadOnlySpan<IQueryAdapter>.Empty;
            }

            return indexQueriesTypes.Length;
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

                // 1. Заполняем типы (capabilities)
                if (!_capabilities.TryGetValue(from, out var typeList))
                {
                    typeList = new List<Type>();
                    _capabilities[from] = typeList;
                }
                typeList.Add(to);

                // 2. Заполняем адаптеры синхронно с типами
                if (!_operationAdapters.TryGetValue(from, out var adapterList))
                {
                    adapterList = new List<IQueryAdapter>();
                    _operationAdapters[from] = adapterList;
                }
                adapterList.Add(adapter);
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
