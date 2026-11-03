using GameServer.Domain.Common.Interfaces;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.Services.StaticData;
using GameServer.Domain.Services.StaticData.Meta;
using GameServer.Domain.SessionWorld.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.StaticResources
{
    public class SessionStaticResources : ISessionStaticResources
    {
        private readonly Dictionary<string, GameObject> _prefabsStorage = new();
        private readonly ILogger<SessionStaticResources> _logger;

        public SessionStaticResources(IStaticResourceLoader staticResourcesLoader, ILogger<SessionStaticResources> logger , IncludedPrefabsRegistryBase prefabsRegister)
        {
            _logger = logger;


            foreach (var gameObject in staticResourcesLoader.LoadResources(prefabsRegister))
            {
                if (_prefabsStorage.ContainsKey(gameObject.Name))
                {
                    _logger.LogError($"prefab: {gameObject} with object nmae: {gameObject.Name} already defined in prefabs storage");
                    continue;
                }

                _prefabsStorage.Add(gameObject.Name, gameObject);
            }
        }

        public GameObject? GetByName(string objectName)
        {
            if(!_prefabsStorage.TryGetValue(objectName, out var result)){
                _logger.LogError($"{objectName} not found");

                return null;
            }

            return result;
        }
    }
}
