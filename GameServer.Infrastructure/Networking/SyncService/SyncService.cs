using GameServer.Application.Interfaces;
using GameServer.Application.Model;
using GameServer.Application.Sessions;
using GameServer.Application.Sessions.Repository;
using GameServer.Contracts.MessageFormats;
using GameServer.Contracts.ServerMessages;
using GameServer.Contracts.WorldMetadata;
using GameServer.Domain.Common.Interfaces;
using GameServer.Domain.Interfaces;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Infrastructure.Serialization;

namespace GameServer.Infrastructure.Networking.SyncService
{
    public class SyncService : ISyncService
    {
        private readonly ISessionBroadcaster _sessionBroadcaster;
        private readonly IGameSessionRepository _sessionRepository;
        private readonly IConnectedClientsStorage _connectedClientStorage;
        private readonly IChunksSettings _chunkSettings;
        private readonly IStaticDataService _staticData;

        public SyncService(ISessionBroadcaster sessionBroadcaster,
            IGameSessionRepository sessionRepository,
            IConnectedClientsStorage connectedClientStorage,
            IChunksSettings chunksSettings,
            IStaticDataService staticDataService)
        {
            _chunkSettings = chunksSettings;
            _sessionBroadcaster = sessionBroadcaster;
            _sessionRepository = sessionRepository;
            _connectedClientStorage = connectedClientStorage;
            _staticData = staticDataService;
        }

        public async Task SyncSessionUsersAsync(Guid sessionId, CancellationToken token = default)
        {
            var session = _sessionRepository.Get(sessionId);
            if (session == null) return;

            var onlinePlayersIds = session.OnlinePlayersIDs;

            var tasks = onlinePlayersIds.Select(id => SyncPlayerWithSession(session, id));

            await Task.WhenAll(tasks);
        }

        private async Task SyncPlayerWithSession(GameSession session, string playerId, CancellationToken token = default)
        {

            var hasClientConnection = _connectedClientStorage.TryGet(playerId, out var clientConnectionObject);
            if (!hasClientConnection) return;

            await clientConnectionObject.ClientReady(token);


            await SendClinetToGlobalSyncState(clientConnectionObject, token);

            await clientConnectionObject.WaitClientActionAsync("ReadyToSync", token);

            Console.WriteLine($"Client ready to Sync: {playerId}");

            var worldObject = session.CurrentRoom;

            await SendConnectionSuccessMessage(clientConnectionObject, clientConnectionObject.UdpToken, token);
            await SendChunkMetaDataAsync(clientConnectionObject, token);
            await SendWorldGenerationMetaDataAsync(clientConnectionObject, worldObject, token);
            await SendWorldDecorationsDataAsync(clientConnectionObject, token);
            await SendPlayersAvatarsDataAsync(clientConnectionObject, worldObject, token);

            await SendMetaDatasDone(clientConnectionObject, token);
        }

        private async Task SendConnectionSuccessMessage(ClientConnection clientConnection, uint udpToken, CancellationToken token)
        {
            var message = new UdpTokenMessage(udpToken);

            _sessionBroadcaster.SendToPlayer(clientConnection, message, MessageFormat.Json);

            await clientConnection.WaitClientActionAsync("Applied", token);
            Console.WriteLine($"Player avatar metaData done: {clientConnection.UserId}");
        }

        private async Task SendMetaDatasDone(ClientConnection clientConnection, CancellationToken token)
        {
            var message = new SyncMetadataDoneMessage();

            _sessionBroadcaster.SendToPlayer(clientConnection, message, MessageFormat.Json);
        }

        private async Task SendPlayersAvatarsDataAsync(ClientConnection clientConnection, GameRoom world, CancellationToken token)
        {
            var message = new PlayerAvatarDataMessage();

            var allPlayers = world.AllPlayers;

            foreach (var player in allPlayers)
            {
                var new_playerDto = new Contracts.PlayerMetaDatas.PlayaerMetaDataDTO
                {
                    PlayerId = player.Id,
                    ObjectId = player.ObjectId,
                    Position = player.Position,
                    IsPlaying = player.Id == clientConnection.UserId
                };

                message.Players.Add(new_playerDto);
            }

            _sessionBroadcaster.SendToPlayer(clientConnection, message, MessageFormat.Json);

            await clientConnection.WaitClientActionAsync("Applied", token);
            Console.WriteLine($"Player avatar metaData done: {clientConnection.UserId}");
        }

        private async Task SendWorldDecorationsDataAsync(ClientConnection clientConnection, CancellationToken token)
        {

            var message = new DecorationRulesMessage
            {
                XmlPayload = _staticData.GetBytes("decorations.xml")
            };

            _sessionBroadcaster.SendToPlayer(clientConnection, message, MessageFormat.Json);

            await clientConnection.WaitClientActionAsync("Applied", token);
            Console.WriteLine($"World decoration metaData done: {clientConnection.UserId}");
        }

        private async Task SendClinetToGlobalSyncState(ClientConnection clientConnection, CancellationToken token)
        {
            var prepareForSyncMessage = new PrepareForSyncMessage();
            _sessionBroadcaster.SendToPlayer(clientConnection, prepareForSyncMessage, Contracts.MessageFormats.MessageFormat.Json);
        }

        private async Task SendWorldGenerationMetaDataAsync(ClientConnection clientConnection, Domain.SessionWorld.GameRoom worldObject, CancellationToken token)
        {
            var chunkGenerator = worldObject.ChunkGenerator;
            var serializer = new GraphSerializer();

            GraphNodeDTO[] flatLandscapeGraph = serializer.SerializeGraph(chunkGenerator.LandscapeRoot);
            GraphNodeDTO[] flatBiomGraph = serializer.SerializeGraph(chunkGenerator.BiomGraphRoot);


            var worldMetaGenerationMetaData = new WorldGenerationMetaDataMessage(flatLandscapeGraph, flatBiomGraph);

            _sessionBroadcaster.SendToPlayer(clientConnection, worldMetaGenerationMetaData, Contracts.MessageFormats.MessageFormat.Json);

            await clientConnection.WaitClientActionAsync("Applied", token);
            Console.WriteLine($"World generation metaData done: {clientConnection.UserId}");
        }

        private async Task SendChunkMetaDataAsync(ClientConnection clientConnection, CancellationToken token)
        {
            ChunkMetaDatasMessage msg = new ChunkMetaDatasMessage
            {
                Pivot = _chunkSettings.Pivot,
                Size = _chunkSettings.ChunkSize
            };

            _sessionBroadcaster.SendToPlayer(clientConnection, msg, Contracts.MessageFormats.MessageFormat.Json);


            await clientConnection.WaitClientActionAsync("Applied", token);
            Console.WriteLine($"chunks metaData done: {clientConnection.UserId}");
        }
    }
}