using GameServer.Domain.Core.GameObjects.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Core.GameObjects
{
    public class GameObject
    {
        public string Name { get; set; }
        public uint ObjectId { get; set; }

        private readonly Dictionary<Type, List<ComponentBase>> _components = new();
        private List<ComponentBase> _componentsArray = new();

        public IEnumerable<ComponentBase> Components { get => _componentsArray; }

        public GameObject(string name)
        {
            Name = name;

            Span<byte> buffer = stackalloc byte[4];
            Random.Shared.NextBytes(buffer);

            ObjectId = BitConverter.ToUInt32(buffer);
        }

        public void AddComponent(ComponentBase component)
        {
            var componentType = component.GetType();

            if (!_components.TryGetValue(componentType, out var componentsList)) {
                componentsList = new();
                _components[componentType] = componentsList;
            }
            _componentsArray.Add(component);

            component.GameObject = this;

            componentsList.Add(component);
        }

        public bool TryGetComponent<T>(out T component)
            where T : ComponentBase
        {
            component = null;

            // Сначала проверяем точное совпадение.
            if (_components.TryGetValue(typeof(T), out var components) &&
                components.Count > 0)
            {
                component = (T)components[0];
                return true;
            }

            // Затем ищем среди наследников.
            foreach (var pair in _components)
            {
                if (!typeof(T).IsAssignableFrom(pair.Key))
                    continue;

                if (pair.Value.Count == 0)
                    continue;

                component = (T)pair.Value[0];
                return true;
            }

            return false;
        }

        public bool TryGetComponents<T>(out List<T> outComponents) where T : ComponentBase
        {
            var type = typeof(T);
            outComponents = null;

            if (!_components.TryGetValue(type, out var result))
            {
                return false;
            }

            outComponents = result.Cast<T>().ToList();
            return true;
        }
    }
}
