using GameServer.Domain.Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Interfaces
{
    public interface IGameObjectFactory
    {
        GameObject CreateByXML(string prefabFile);
    }
}
