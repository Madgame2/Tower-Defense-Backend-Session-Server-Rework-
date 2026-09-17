using GameServer.Domain.ColliderSystem.Enum;
using GameServer.Domain.Player;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.Events.Stateshandler;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.Generators;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Routers;
using GameServer.Domain.SessionWorld.WorldQuery.Interfaces;
using System.Numerics;
using EntityLib.GameRoomExtentions;
using GameServer.Application.Components.Common;

namespace GameServer.Application.Sessions.States
{
    [Transition(typeof(SyncState))]
    public class GenerateWorldState : BaseState
    {
        private readonly IChunksSettings _chunksSettings;
        private readonly IWorldQueryService _worldQueryService;
        private IChunkGeneratorFactory _chankGeneratorFactory;
        private ILandscapeGraphFactory _landscapeGraphFactory;
        private IBiomGraphFactory _biomGraphFactory;

        public GenerateWorldState(IChunksSettings chunksSettings, IWorldQueryService worldQueryService,
            IChunkGeneratorFactory chankFactory, ILandscapeGraphFactory landscapeGraphFactory, IBiomGraphFactory biomGraphFactory)
        {
            _worldQueryService = worldQueryService;
            _chunksSettings = chunksSettings;
            _chankGeneratorFactory = chankFactory;
            _landscapeGraphFactory = landscapeGraphFactory;
            _biomGraphFactory = biomGraphFactory;
        }

        public override void Configure(StateEventHandler handlers)
        {
            base.Configure(handlers);

            handlers.Register<PlayerConnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player join {evt.PlayerId}");
            });

            handlers.Register<PlayerDisconnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player leave {evt.PlayerId}");
            });
        }


        public override async Task OnEnter(GameSession ctx)
        {
            Console.WriteLine("ON GenerateWorld");

            var router = new GeneralMathcRouter();
            var newWorldObject = new GameRoom(_chunksSettings, _worldQueryService, router);

            CreateWorldGenerationRules(newWorldObject);
            await SpawnPlayers(ctx.OnlinePlayersIDs, newWorldObject);

            ctx.CurrentRoom = newWorldObject;

            var tereeDebugEnity = newWorldObject.CreateEntity();
            newWorldObject.GetStash<PositionComponent>().Set(tereeDebugEnity, new PositionComponent());

            await EnterSyncState(ctx);
        }

        private void CreateWorldGenerationRules(GameRoom newWorldObject)
        {
            var chunkGenerator = CreateChunkGenerator();
            newWorldObject.ChunkGenerator = chunkGenerator;
        }

        private SessionChunkGenerator CreateChunkGenerator()
        {
            var landscapeGraph = _landscapeGraphFactory.Create(53523256262632);
            var biomeGraph = _biomGraphFactory.Create(53523256262632);
            return _chankGeneratorFactory.CreateForSession(landscapeGraph, biomeGraph);
        }

        private async Task SpawnPlayers(HashSet<string> onlinePlayersIDs, GameRoom world)
        {
            foreach (var player in onlinePlayersIDs)
            {

                var y = await world.GetHeightAt(0, 0);

                var playerPosition = new Vector3(0, y, 0);

                var playerobj = new Player(player, playerPosition);

                playerobj.Size = new Vector3(1, 2, 1);

                playerobj.Colider.Type = ColliderType.Capsule;
                playerobj.Colider.Height = 2;
                playerobj.Colider.Radius = 0.5f;

                playerobj.Pivot = new Vector3(0, -1, 0);

                world.RegPlayer(playerobj);
            }
        }

        private static Task EnterSyncState(GameSession session)
        {
            return session.StateMachine.MoveTo<SyncState>();
        }
    }
}
