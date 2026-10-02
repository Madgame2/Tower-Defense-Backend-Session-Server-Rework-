using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters
{
    public readonly struct NamedParameter<T>: IQueryParameter<T>
    {
        private readonly string _name;

        public NamedParameter(string name)
        {
            _name = name;
        }
    }
}
