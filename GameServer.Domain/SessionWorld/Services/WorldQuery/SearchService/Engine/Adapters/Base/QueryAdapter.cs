using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base
{
    public abstract class QueryAdapter<TFrom, TTo> :  IQueryAdapter
    {
        public Type From => typeof(TFrom);
        public Type To => typeof(TTo);

        public abstract TTo Adapt(TFrom query);
    }
}
