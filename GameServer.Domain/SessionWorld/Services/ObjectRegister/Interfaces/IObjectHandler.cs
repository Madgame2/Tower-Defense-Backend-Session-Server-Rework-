using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces
{

    public interface IObjectHandler {
        
        public Type StoragedType {  get; }
    }

    public interface IObjectHandler<T> : IObjectHandler
    {
        public Type StoragedType => typeof(T);

        void Add(T newObject);
        void Change(T obj);
        void Remove(T obj);

        ReadOnlySpan<T> GetAll();
    }

    public interface IObjectHandler<T, TSearch> : IObjectHandler<T>
    {
        T? Get(TSearch arg);
    }
}
