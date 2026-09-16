using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage;
using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage.Interfaces;
using GameServer.Domain.SessionWorld.Generators;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.PlayerStorages;
using GameServer.Domain.SessionWorld.PlayerStorages.Interfaces;
using GameServer.Domain.SessionWorld.WorldQuery.Interfaces;
using GameServer.Domain.ValueObjects;
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

        public IPacketRouter Router { get => _packetRouter; }

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
            IWorldQueryService worldQueryService,
            IPacketRouter packetRouter)
        {
            _chunksSettings = chunksSettings;
            _worldQueryService = worldQueryService;
            _packetRouter = packetRouter;

            _chunkStorage = new InMemmoryChunkStorage();
            _playersStorage = new InMemmoryPlayerStorage();
        }

        public async Task<float> GetHeightAt(float WorldX, float WorldZ)
        {
            return await _worldQueryService.GetHeightAt(this, WorldX, WorldZ);
        }

        public async Task<Chank> GetOrGenerateChunkAsync(float worldX, float worldZ)
        {
            var chunkSize = _chunksSettings.ChunkSize;
            var chunkPivot = _chunksSettings.Pivot;

            float adjustedX = worldX + (chunkSize * chunkPivot.X);
            float adjustedZ = worldZ + (chunkSize * chunkPivot.Y);

            float chunkGridX = MathF.Floor(adjustedX / chunkSize);
            float chunkGridZ = MathF.Floor(adjustedZ / chunkSize);

            float pivotX = chunkGridX;
            float pivotZ = chunkGridZ;

            var pivot = new Vector2(pivotX, pivotZ);

            var chunk = _chunkStorage.Get(pivot);
            if (chunk != null)
            {
                return chunk;
            }

            chunk = await _chunkGenerator.CreateChankAsync(pivot);

            _chunkStorage.Save(chunk);

            return chunk;
        }

        public void RegPlayer(Player.Player playerobj)
        {
            _playersStorage.Add(playerobj);
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
    }
}
