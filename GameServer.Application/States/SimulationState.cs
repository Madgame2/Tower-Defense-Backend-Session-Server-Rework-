using GameServer.Domain.Sessions;
using Microsoft.Extensions.Logging;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.GameLoop.Core;
using GameServer.GameLoop.Core.Simultaion.Factory.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Sessions.States
{
    public class SimulationState : BaseState
    {
        private readonly ISimulationFactory _simulationFactory;
        private readonly ILogger<SimulationState> _logger;

        public SimulationState(ISimulationFactory simulationFactory,
            ILogger<SimulationState> logger)
        {
            _simulationFactory = simulationFactory;
            _logger = logger;
        }

        public override async Task OnEnter(GameSession ctx)
        {
            Console.WriteLine("ON Simulation State");

            try
            {
                var generalGameModeConfig = new GeneralSessionConfig();

                ISimulation worldSimulation = _simulationFactory.Create(ctx.CurrentRoom, generalGameModeConfig);

                ctx.Simulation = worldSimulation;

                worldSimulation.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            } 
        }
    }
}
