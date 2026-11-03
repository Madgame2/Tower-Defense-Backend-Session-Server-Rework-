using GameServer.Domain.Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.GameObjectSpawner.Modules.Base
{
    internal interface IGameObjectSpawnModule<T> where T: GameObject
    {
        void Configure(T gameObject);
    }
}
