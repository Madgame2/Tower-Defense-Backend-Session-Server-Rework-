using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface IObjectRegistry
    {
        void RegisterHandler<T>(IObjectHandler<T> handler);

        void Add<T>(T obj);
        void Remove<T>(T obj);
        void Change<T>(T obj);
    }
}
