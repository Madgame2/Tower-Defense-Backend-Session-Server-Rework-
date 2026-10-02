using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters
{
    public readonly struct ConstParameter<T>: IQueryParameter<T>
    {
        private readonly T _value;

        public ConstParameter(T value)
        {
            _value = value;
        }
    }
}
