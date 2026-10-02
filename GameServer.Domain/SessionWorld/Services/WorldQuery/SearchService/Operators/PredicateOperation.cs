using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators
{
    public sealed class PredicateOperation<T> : IQueryOperation
    {
        public Func<T, bool> Predicate { get; }

        public PredicateOperation(Func<T, bool> predicate)
        {
            Predicate = predicate;
        }

        public IEnumerable<IQueryParameter> GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
