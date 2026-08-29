using GameServer.Domain.SessionWorld;
using GameServer.GameLoop.Core.Simultaion.Interfaces;
using GameServer.GameLoop.Core.Simultaion.Meta.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Simultaion.Factory.Interfaces
{
    public interface ISimulationFactory
    {
        ISimulation Create(GameRoom world, SimulationConfig config);
    }
}
