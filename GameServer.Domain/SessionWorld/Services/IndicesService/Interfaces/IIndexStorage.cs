using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces
{
    public interface IIndexStorage
    {
        void Add<T, TQuery>(
            IIndex<T, TQuery> index);

        IIndex<T, TQuery> Get<T, TQuery>();
    }
}
