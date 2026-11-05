using GameServer.Domain.SessionWorld;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Simultaion.Steps.Interfaces
{
    public interface INetworkTickable
    {
        void OnAwake(GameRoom world);

        void NetworkTick(float delta,uint Tick, GameRoom world);
    }
}
