using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces
{

    public interface IObjectHandler { }

    public interface IObjectHandler<T> : IObjectHandler
    {
        void Add(T newObject);
        void Change(T obj);
        void Remove(T obj);
    }
}
