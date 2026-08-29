using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Simultaion.Meta.Base
{
    public abstract class SimulationConfig
    {
        private readonly List<Type> _executionOrder = new();
        private readonly List<Type> _networkOrder = new();

        public IReadOnlyList<Type> ExecutionOrder => _executionOrder;
        public IReadOnlyList<Type> NetworkOrder => _networkOrder;
        public abstract void Configure();

        protected SimulationConfig Register<T>() where T : ITickable
        {
            if (_executionOrder.Contains(typeof(T)))
            {
                throw new InvalidOperationException($"Система {typeof(T).Name} уже зарегистрирована.");
            }

            _executionOrder.Add(typeof(T));
            return this;
        }

        protected SimulationConfig RegisterNetwork<T>() where T : INetworkTickable
        {
            if (_networkOrder.Contains(typeof(T)))
            {
                throw new InvalidOperationException($"Система {typeof(T).Name} уже зарегистрирована.");
            }

            _networkOrder.Add(typeof(T));
            return this;
        }
    }
}
