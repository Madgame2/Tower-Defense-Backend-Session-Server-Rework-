using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces
{
    public interface IIndex : IDisposable
    {
    }

    public interface IIndex<T> : IIndex
    {
        void Add(T item);
        void Remove(T item);
    }

    public interface IIndex<T, TQuery> : IIndex<T>
    {
        void Get(TQuery query, List<T> result);
    }
}
