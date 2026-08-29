using GameServer.Domain.Common.Model;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Meta
{
    public class WorldSettings : IChunksSettings
    {
        public int ChunkSize { get; set; }
        public Vector2Config Pivot { get; set; }

        Vector2 IChunksSettings.Pivot => new Vector2(Pivot.X, Pivot.Y);
    }
}
