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

            component.GameObject = this;

            componentsList.Add(component);
        }

        public bool TryGetComponent<T>(out T component)
            where T : ComponentBase
        {
            component = null;

            if (!_components.TryGetValue(typeof(T), out var components))
                return false;

            if (components.Count == 0)
                return false;

            component = (T)components[0];
            return true;
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
