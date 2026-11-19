using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester;
using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model
{
    public class QueryContext<T>
    {
        private readonly IIndexStorage _indexStorage;
        private readonly IObjectRegistry _objectRegistry;
        private readonly IObjectRegesterContext _objectregisterContext;

        public IObjectRegesterContext ObjectregisterContext=> _objectregisterContext;
        public IIndexStorage Indexes => _indexStorage;
        public IObjectRegistry ObjectsRegistry => _objectRegistry;

        private Stack<List<T>> _evaluationStack = new Stack<List<T>>(capacity: 6);

        private Dictionary<string, IQueryParameter> _namedParameters = new();

        public QueryContext(IIndexStorage indexStorage, IObjectRegistry objectRegistry, IObjectRegesterContext regesterContext)
        {
            _indexStorage = indexStorage;
            _objectRegistry = objectRegistry;
            _objectregisterContext = regesterContext;
        }

        public void Set<T>(string argName, T Value)
        {
            if (!_namedParameters.TryGetValue(argName, out var parameter))
            {
                throw new ArgumentException($"{argName} not definded variable name in this query context");
            }
            if(parameter is not NamedParameter<T> namedParameter)
            {
                throw new ArgumentException($"{argName} has wrong type");
            }

            namedParameter.Value = Value;
        }

        internal void Link(IQueryOperation operation, IEnumerable<IQueryParameter> parameters)
        {
            foreach (var parametr in parameters)
            {
                if (parametr is NamedParameter namedParameter)
                {
                    if (_namedParameters.ContainsKey(namedParameter.Name))
                    {
                        throw new ArgumentException($"{namedParameter.Name} already definded variable in this query context");
                    }

                    _namedParameters.Add(namedParameter.Name, namedParameter);
                }
            }
        }
    }
}
