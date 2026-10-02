using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base
{
    public interface IQueryParameter
    {
    }

    public interface IQueryParameter<T>: IQueryParameter
    {
    }
}
