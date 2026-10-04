using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces
{

    public delegate void IndexExecutorDelegate<T>(
        IIndex<T> index,
        IQueryOperation operation,
        QueryContext context,
        List<T> buffer);

    public readonly struct IndexEntry<T>
    {
        public readonly IIndex<T> Index;
        public readonly IndexExecutorDelegate<T> Executor;

        public IndexEntry(IIndex<T> index, IndexExecutorDelegate<T> executor)
        {
            Index = index;
            Executor = executor;
        }
    }

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
