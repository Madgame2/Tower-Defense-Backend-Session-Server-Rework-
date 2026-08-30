using GameServer.GameLoop.Core.Simultaion.Meta.Base;
using GameServer.GameLoop.Core.Systems;
using GameServer.GameLoop.Core.Systems.ResolveSystems;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core
{
    public class GeneralSessionConfig : SimulationConfig
    {
        public override void Configure()
        {
            Register<ApplyPlayerMoveInputSystem>();
            Register<GravitySystem>();
            Register<VelocityToPositionSystem>();
            Register<VertivalResolveSystem>();

            RegisterNetwork<StateBroadcastSystem>();
        }
    }
}