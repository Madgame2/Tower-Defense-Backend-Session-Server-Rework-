using GameServer.Domain.Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface IGameObjectSpawner
    {
        T Spawn<T>() where T : GameObject;
        T Spawn<T>(params object[] parameters) where T : GameObject;
    }
}
