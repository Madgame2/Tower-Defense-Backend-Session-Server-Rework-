using GameServer.Domain.SessionWorld;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameLoop.Core.Simultaion.Steps.Interfaces
{
    public interface ITickable
    {
        Task Tick(float delta, GameRoom world);
    }
}
