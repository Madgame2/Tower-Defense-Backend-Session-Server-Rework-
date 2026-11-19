using GameServer.Domain.ColliderSystem.Enum;
using GameServer.Domain.Interfaces;
using GameServer.Domain.Player;
using GameServer.Domain.Sessions;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.Events.Stateshandler;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.GameLoop.Core;
using GameServer.GameLoop.Core.Simultaion.Factory.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Meta;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Sessions.States
{
    public class SimulationState : BaseState
    {
        private readonly ISimulationFactory _simulationFactory;
        private readonly ILogger<SimulationState> _logger;
        private readonly ISyncService _syncService;

        public SimulationState(ISimulationFactory simulationFactory,
            ILogger<SimulationState> logger,
            ISyncService syncService)
        {
            _simulationFactory = simulationFactory;
            _logger = logger;
            _syncService = syncService;
        }

        public override void Configure(StateEventHandler handlers)
        {
            base.Configure(handlers);

            handlers.Register<PlayerConnected>(async (ctx, evt) =>
            {
                //Console.WriteLine($"Player join {evt.UserId}");
            });

        }

        public override async Task OnEnter(GameSession ctx)
        {
            //Console.WriteLine("ON Simulation State");

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
