using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.Services.StaticData.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Services.StaticData
{
    public interface IStaticResourceLoader
    {
        IEnumerable<GameObject> LoadResources(IncludedPrefabsRegistryBase prefabsRegister, Action<Exception> onError = null);
    }
}
