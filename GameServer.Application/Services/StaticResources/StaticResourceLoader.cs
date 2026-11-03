using GameServer.Application.Services.Factories.GameObjectFactory.Interfaces;
using GameServer.Domain.Common.Interfaces;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.Services.StaticData;
using GameServer.Domain.Services.StaticData.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.StaticResources
{
    public class StaticResourceLoader : IStaticResourceLoader
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IGameObjectFactory _gameObjectFactory;

        public StaticResourceLoader(IStaticDataService staticDataService, IGameObjectFactory gameObjectFactory)
        {
            _staticDataService = staticDataService;
            _gameObjectFactory = gameObjectFactory;
        }

        public IEnumerable<GameObject> LoadResources(IncludedPrefabsRegistryBase prefabsRegister, Action<Exception> onError = null)
        {
            prefabsRegister.Incleded();

            foreach (var prefabFilePath in prefabsRegister)
            {
                GameObject createdGameObject;
                try
                {
                    var prefabFile = _staticDataService.GetText(prefabFilePath);
                    createdGameObject = _gameObjectFactory.CreateByXML(prefabFile);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                    continue;
                }

                yield return createdGameObject;
            }
        }
    }
}
