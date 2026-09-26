using GameServer.Domain.SessionWorld.Services.IndicesService.Queries.Base;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.Queries
{
    public record struct SpatiialGridQuery
    {
        public Vector2 position;
        public int radius;
    }
}
