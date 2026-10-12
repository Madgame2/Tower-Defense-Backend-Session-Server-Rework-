using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters
{

    public class NamedParameter : IQueryParameter
    {
        protected string _name;

        public string Name => _name;
    }

    public class NamedParameter<T>: NamedParameter, IQueryParameter<T>
    {
        public T Value { get; set; }

        public NamedParameter(string name)
        {
            _name = name;
        }

        public T GetValue() => Value;
    }
}
