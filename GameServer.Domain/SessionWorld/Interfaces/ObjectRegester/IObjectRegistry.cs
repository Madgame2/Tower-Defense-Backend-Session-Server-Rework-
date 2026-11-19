using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester
{
    public interface IObjectRegistry
    {
        void RegisterHandler(IObjectHandler handler);
        void Add<T>(T obj);
        void Remove<T>(T obj);
        void Change<T>(T obj);
        TResult? Get<TResult, TSearch>(TSearch arg);

        ReadOnlySpan<T> GetAll<T>();

    }
}
