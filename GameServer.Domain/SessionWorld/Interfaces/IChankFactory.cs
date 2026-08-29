using GameServer.Domain.SessionWorld.Model;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces
{
    public interface IChankFactory
    {
        Task<Chank> CreateChankAsync(Vector2 offset);
    }
}
