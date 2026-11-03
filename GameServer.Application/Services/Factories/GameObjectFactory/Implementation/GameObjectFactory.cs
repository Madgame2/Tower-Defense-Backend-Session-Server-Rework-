using GameServer.Application.Services.Factories.GameObjectFactory.DTO;
using GameServer.Application.Services.Factories.GameObjectFactory.Interfaces;
using GameServer.Application.Services.Factories.GameObjectFactory.Modules.Base;
using GameServer.Domain.Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Implementation
{
    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly Dictionary<string, GameObjectComponentFactoryModule> _modules = new();
        private readonly XmlSerializer _serializer = new(typeof(NetObjectData));
        public GameObjectFactory(IEnumerable<GameObjectComponentFactoryModule> modules)
        {
            foreach (var module in modules)
            {
                _modules.Add(module.Component, module);
            }
        }

        public GameObject CreateByXML(string prefabFile)
        {
            var document = XDocument.Parse(prefabFile);

            var root = document.Root
                ?? throw new FormatException("NetObject root element is missing.");

            var objectName = root
                .Element("Metadata")?
                .Element("ObjectName")?
                .Value
                ?? throw new FormatException("ObjectName is missing.");

            var gameObject = new GameObject(objectName);

            var componentsElement = root.Element("Components");

            if (componentsElement is null)
                return gameObject;

            foreach (var element in componentsElement.Elements("Component"))
            {
                var type = (string?)element.Attribute("type")
                    ?? throw new FormatException("Component type is missing.");

                var componentData = new ComponentData
                {
                    Type = type,
                    Metadata = element.Element("Metadata")
                        ?? throw new FormatException(
                            $"Metadata is missing for component '{type}'."),

                    Data = element.Element("Data")
                        ?? throw new FormatException(
                            $"Data is missing for component '{type}'.")
                };

                if (!_modules.TryGetValue(type, out var module))
                {
                    throw new InvalidOperationException(
                        $"Factory module not found for component: {type}");
                }

                var component = module.CreateByXML(componentData);
                gameObject.AddComponent(component);
            }

            return gameObject;
        }
    }
}
