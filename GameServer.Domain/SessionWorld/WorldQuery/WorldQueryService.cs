using GameServer.Domain.SessionWorld.WorldQuery.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.WorldQuery
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

            int x0 = (int)MathF.Floor(localX);
            int z0 = (int)MathF.Floor(localZ);

            x0 = Math.Clamp(x0, 0, chunk.Size - 1);
            z0 = Math.Clamp(z0, 0, chunk.Size - 1);

            int x1 = x0 + 1;
            int z1 = z0 + 1;

            float h00 = chunk.GetLandscapeHeight(x0, z0);
            float h10 = chunk.GetLandscapeHeight(x1, z0);
            float h01 = chunk.GetLandscapeHeight(x0, z1);
            float h11 = chunk.GetLandscapeHeight(x1, z1);

            float fx = localX - MathF.Floor(localX);
            float fz = localZ - MathF.Floor(localZ);

            if (fx + fz <= 1.0f)
            {
                return h00
                    + fx * (h10 - h00)
                    + fz * (h01 - h00);
            }

            return h11
                + (1.0f - fx) * (h01 - h11)
                + (1.0f - fz) * (h10 - h11);
        }
    }
}
