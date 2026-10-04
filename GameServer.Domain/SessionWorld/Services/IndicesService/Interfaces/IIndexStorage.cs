using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces
{
    public interface IIndexStorage
    {
        void Add<T, TQuery>(IIndex<T, TQuery> index);
        ReadOnlySpan<IIndex<T>> Get<T>();
        ReadOnlySpan<IndexEntry<T>> Get<T, TQuery>();
        ReadOnlySpan<IndexEntry<T>> Get<T>(Type queryType);
    }
}
