using GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery
{
    public class WorldQueryService : IWorldQueryService
    {
        public async Task<float> GetHeightAt(GameRoom world, float worldX, float worldZ)
        {
            var chunk = await world.GetOrGenerateChunkAsync(worldX, worldZ);

            float chunkSize = chunk.Size;

            float chunkIndexX = chunk.Position.X;
            float chunkIndexZ = chunk.Position.Y;

            float chunkPivotWorldX = chunkIndexX * chunkSize;
            float chunkPivotWorldZ = chunkIndexZ * chunkSize;

            float chunkLeftEdgeX =
                chunkPivotWorldX - chunkSize * chunk.Pivot.X;

            float chunkLeftEdgeZ =
                chunkPivotWorldZ - chunkSize * chunk.Pivot.Y;

            float localX = worldX - chunkLeftEdgeX;
            float localZ = worldZ - chunkLeftEdgeZ;

            float clampedX = Math.Clamp(localX, 0f, chunk.Size);
            float clampedZ = Math.Clamp(localZ, 0f, chunk.Size);

            int x0 = (int)MathF.Floor(clampedX);
            int z0 = (int)MathF.Floor(clampedZ);

            if (x0 >= chunk.Size) x0 = chunk.Size - 1;
            if (z0 >= chunk.Size) z0 = chunk.Size - 1;


            int x1 = x0 + 1;
            int z1 = z0 + 1;

            float fx = clampedX - x0;
            float fz = clampedZ - z0;

            float h00 = chunk.GetLandscapeHeight(x0, z0);
            float h10 = chunk.GetLandscapeHeight(x1, z0);
            float h01 = chunk.GetLandscapeHeight(x0, z1);
            float h11 = chunk.GetLandscapeHeight(x1, z1);

            float calculatedHeight;

            if (fx + fz <= 1.0f)
            {
                calculatedHeight = h00
                    + fx * (h10 - h00)
                    + fz * (h01 - h00);
            }

            calculatedHeight = h11
                + (1.0f - fx) * (h01 - h11)
                + (1.0f - fz) * (h10 - h11);

            return calculatedHeight;
        }
    }
}
