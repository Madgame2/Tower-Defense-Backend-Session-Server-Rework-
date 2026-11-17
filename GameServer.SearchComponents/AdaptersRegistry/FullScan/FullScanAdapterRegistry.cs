using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.AdaptersRegistry.Index.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameServer.SearchComponents.AdaptersRegistry.FullScan
{
    public class FullScanAdapterRegistry : IFullScanAdapterRegistry
    {
        private readonly Dictionary<(Type Operation, Type Data), IFullScanAdapter>
            _adapters = new();

        public FullScanAdapterRegistry(IEnumerable<IFullScanAdapter> adapters)
        {
            foreach (var adapter in adapters)
            {
                var key = (OperationType: adapter.OperationType,
                            DataType: adapter.DataType);

                if (!_adapters.TryAdd(key, adapter))
                {
                    throw new InvalidOperationException(
                        $"FullScan adapter for '{key.OperationType.Name}' " +
                        $"and '{key.DataType.Name}' is already registered.");
                }
            }
        }

        public bool TryResolve<T>(
            IQueryOperation operation,
            out IFullScanAdapter adapter)
        {
            return _adapters.TryGetValue(
                (operation.GetType(), typeof(T)),
                out adapter!);
        }
    }
}
