using GameServer.Domain.ColliderSystem.Core;
using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage;
using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage.Interfaces;
using GameServer.Domain.SessionWorld.Generators;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.PlayerStorages;
using GameServer.Domain.SessionWorld.PlayerStorages.Interfaces;
using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.IndicesService.Storages;
using GameServer.Domain.SessionWorld.Services.ObjectRegister;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Handlers;
using GameServer.Domain.SessionWorld.Services.WorldQuery;
using GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace GameServer.Domain.SessionWorld
{
    public class GameRoom
    {
        private SessionChunkGenerator _chunkGenerator;
        private readonly IChunkStorage _chunkStorage;
        private readonly IChunksSettings _chunksSettings;
        private readonly IPlayersStorage _playersStorage;
        private readonly IWorldQueryService _worldQueryService;
        private readonly IPacketRouter _packetRouter;
        private readonly IIndexStorage _indicesStorage;

        private readonly IObjectRegistry _objectRegistry;

        public IPacketRouter Router { get => _packetRouter; }
        public IObjectRegistry ObjectRegistry { get => _objectRegistry; }

        public IWorldQueryService worldQueryService { get => _worldQueryService; }

        public SessionChunkGenerator ChunkGenerator
        {
            get => _chunkGenerator;
            set
            {
                if (_chunkGenerator == null)
                    _chunkGenerator = value;
            }
        }

        public Player.Player[] AllPlayers { get => _playersStorage.GetAll(); }

        public GameRoom(IChunksSettings chunksSettings,
            IPacketRouter packetRouter,
            IQueryExecutor queryExecuteEngine)
        {
            _chunksSettings = chunksSettings;
            _packetRouter = packetRouter;

            _worldQueryService = new WorldQueryService(this, queryExecuteEngine);
            _chunkStorage = new InMemmoryChunkStorage();
            _playersStorage = new InMemmoryPlayerStorage();
            _indicesStorage = new InMemmoryIndesStorage();
            _objectRegistry = new InMemmoryObjectRegister();

            var contex = new GameRoomContext(_chunkStorage, chunksSettings, _playersStorage, _indicesStorage);

            _objectRegistry.RegisterHandler(new PlayersRegisterHandler(_playersStorage));
        }

        public void AddIndex<T, TQuery>(IIndex<T, TQuery> indexsystem)
        {
            _indicesStorage.Add(indexsystem);
        }


        public async Task<Chank> GetOrGenerateChunkAsync(float worldX, float worldZ)
        {
            var pivot = CalculateChunkPivot(worldX, worldZ);

            var chunk = _chunkStorage.Get(pivot);

            if (chunk != null)
                return chunk;

            chunk = await _chunkGenerator.CreateChankAsync(pivot);

            _objectRegistry.Add(chunk);

            return chunk;
        }

        private Vector2 CalculateChunkPivot(float worldX, float worldZ)
        {
            var chunkSize = _chunksSettings.ChunkSize;
            var chunkPivot = _chunksSettings.Pivot;

            float adjustedX = worldX + (chunkSize * chunkPivot.X);
            float adjustedZ = worldZ + (chunkSize * chunkPivot.Y);

            float chunkGridX = MathF.Floor(adjustedX / chunkSize);
            float chunkGridZ = MathF.Floor(adjustedZ / chunkSize);

            float pivotX = chunkGridX;
            float pivotZ = chunkGridZ;

            return new Vector2(pivotX, pivotZ);
        }

        public void HandleMoveInputs(string playerId, uint lastTick, ReadOnlySpan<MoveInputCommand> inputs)
        {
            var player = _playersStorage.Get(playerId);

            if (player == null)
                return;

            foreach (var input in inputs)
            {
                player.InputBuffer.AddInput(input);
            }
        }

        public class GameRoomContext
        {
            public IChunkStorage ChunkStorage { get; }
            public IChunksSettings ChunksSettings { get; }
            public IPlayersStorage PlayersStorage { get; }
            public IIndexStorage IndicesStorage { get; }

            public GameRoomContext(IChunkStorage chunkStorage,
                IChunksSettings chunksSettings,
                IPlayersStorage playersStorage,
                IIndexStorage indexStorage)
            {
                ChunksSettings = chunksSettings;
                ChunksSettings = chunksSettings;
                PlayersStorage = playersStorage;
                IndicesStorage = indexStorage;
            }
        }
    }
}
