using GameServer.Domain.ColliderSystem.Enum;
using GameServer.Domain.Services.StaticData;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Builder;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using System.Numerics;
using System.Security.Cryptography;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery
{
    public class WorldQueryService : IWorldQueryService
    {
        private GameRoom _gameRoom;
        private readonly IQueryPlanner _queryPlanner;
        private readonly IQueryExecutor _queryExecutor;
        private readonly ISessionStaticResources _staticResources;

        public GameRoom GameRoom
        {
            set
            {
                if (_gameRoom == null && value != null)
                    _gameRoom = value;

                return;
            }
        }

        public WorldQueryService(
            IQueryPlanner queryPlanner,
            IQueryExecutor queryExecutor,
            ISessionStaticResources staticRessources)
        {
            _queryPlanner = queryPlanner;
            _queryExecutor = queryExecutor;
            _staticResources = staticRessources;
        }

        public async Task<float> GetHeightAt(float worldX, float worldZ)
        {
            var chunk = await _gameRoom.GetOrGenerateChunkAsync(worldX, worldZ);

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

        public WorldQueryBuilder<T> Search<T>()
        {
            return new WorldQueryBuilder<T>(_gameRoom, _queryPlanner);
        }

        public void ExecuteQuery<T>(QueryExecutePlan<T> plan, QueryContext<T> queryContext, List<T> buffer)
        {
            buffer.Clear();
            _queryExecutor.Execute(plan, queryContext, buffer);
        }

        public async Task<Player.Player> RegisterPlayerAsync(string userID, CancellationToken cancellationToken)
        {
            var existingPlayer = _gameRoom.ObjectRegistry.Get<Player.Player,string>(userID);

            if (existingPlayer != null)
                return existingPlayer;

            Span<byte> bytes = stackalloc byte[4];

            RandomNumberGenerator.Fill(bytes);

            uint playerID = BitConverter.ToUInt32(bytes);

            var playerPrefab = _staticResources.GetByName("Skadi_character");
            var player = _gameRoom.GameObjectSpawner.Spawn<Player.Player>(playerPrefab,userID, playerID);

            player.LinkedRoom = _gameRoom;

            var y = await GetHeightAt(0, 0);

            cancellationToken.ThrowIfCancellationRequested();

            if (player.TryGetComponent<TransformComponent>(out var transformCompoent))
            {
                transformCompoent.Position = new Vector3(0, y, 0);
            }

            player.Size = new Vector3(1, 2, 1);

            player.Colider.Type = ColliderType.Capsule;
            player.Colider.Height = 2;
            player.Colider.Radius = 0.5f;

            player.Pivot = new Vector3(0, -1, 0);

            _gameRoom.ObjectRegistry.Add(player);

            return player;
        }
    }
}
