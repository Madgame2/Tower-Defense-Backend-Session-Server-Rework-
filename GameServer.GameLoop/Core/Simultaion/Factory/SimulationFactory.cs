using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Factory.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Meta.Base;
using GameServer.GameLoop.Core.Simultaion.Steps.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Simultaion.Factory
{
    public class SimulationFactory : ISimulationFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SimulationFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISimulation Create(GameRoom world, SimulationConfig config)
        {
            config.Configure();

            List<ITickable> systems = new List<ITickable>();
            List<INetworkTickable> networkTickables = new List<INetworkTickable>();
            foreach (var systemType in config.ExecutionOrder)
            {
                var system = (ITickable)ActivatorUtilities.CreateInstance(_serviceProvider, systemType);
                systems.Add(system);
            }

            foreach (var systemType in config.NetworkOrder)
            {
                var system = (INetworkTickable)ActivatorUtilities.CreateInstance(_serviceProvider, systemType);
                networkTickables.Add(system);
            }

            return new Simulation(world, systems, networkTickables);
        }
    }
}
