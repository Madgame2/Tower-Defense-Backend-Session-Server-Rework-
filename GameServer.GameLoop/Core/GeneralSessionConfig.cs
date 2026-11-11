using GameServer.GameLoop.Core.Simultaion.Meta.Base;
using GameServer.GameLoop.Core.Systems;
using GameServer.GameLoop.Core.Systems.GamePlay;
using GameServer.GameLoop.Core.Systems.NetworkSystems;
using GameServer.GameLoop.Core.Systems.Replication;
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
            Register<ApplyPlayerInputSystem>();

            Register<MovementSystem>();
            Register<ProcessPlayerJumpSystem>();
            Register<RotatePlayerSystem>();
            Register<SetPlayerLookAtSystem>();
            Register<ProcessAlternativePlayerActionSystem>();

            //Register<ProcessJumpSystem>();
            Register<GravitySystem>();
            Register<VelocityToPositionSystem>();
            Register<VertivalResolveSystem>();
            Register<HorizontalResolveSystem>();
            Register<PlayerMoveStateResolveSystem>();

            Register<SaveEventsSystem>();

            RegisterNetwork<WorldReplicationBroadcastSystem>();
            RegisterNetwork<StateBroadcastSystem>();
        }
    }
}