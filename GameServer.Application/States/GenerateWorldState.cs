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
using System.Numerics;
using EntityLib.GameRoomExtentions;
using GameServer.Application.Components.Common;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph.Factory.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.IndicesService.IndexesImplement;

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
        private ITreeGraphFactory _treeGraphFactory;

        public GenerateWorldState(IChunksSettings chunksSettings,
            IWorldQueryService worldQueryService,
            IChunkGeneratorFactory chankFactory,
            ILandscapeGraphFactory landscapeGraphFactory,
            IBiomGraphFactory biomGraphFactory,
            ITreeGraphFactory treeGraphFactory)
        {
            _worldQueryService = worldQueryService;
            _chunksSettings = chunksSettings;
            _chankGeneratorFactory = chankFactory;
            _landscapeGraphFactory = landscapeGraphFactory;
            _biomGraphFactory = biomGraphFactory;
            _treeGraphFactory = treeGraphFactory;
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

            var treesIndex = new SpatialGridIndex<StaticTreeData>((treeData) => { return treeData.LocalPosition; });
            newWorldObject.AddIndex(treesIndex);

            CreateWorldGenerationRules(newWorldObject);
            await SpawnPlayers(ctx.OnlinePlayersIDs, newWorldObject);

            ctx.CurrentRoom = newWorldObject;

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
            var treeGraph = _treeGraphFactory.Create(53523256262632);
            return _chankGeneratorFactory.CreateForSession(landscapeGraph, biomeGraph, treeGraph);
        }

        private async Task SpawnPlayers(HashSet<string> onlinePlayersIDs, GameRoom world)
        {
            foreach (var player in onlinePlayersIDs)
            {

                var y = await world.worldQueryService.GetHeightAt(world,0, 0);

                var playerPosition = new Vector3(0, y, 0);

                var playerobj = new Player(player, playerPosition);

                playerobj.Size = new Vector3(1, 2, 1);

                playerobj.Colider.Type = ColliderType.Capsule;
                playerobj.Colider.Height = 2;
                playerobj.Colider.Radius = 0.5f;

                playerobj.Pivot = new Vector3(0, -1, 0);

                world.ObjectRegistry.Add(playerobj);
            }
        }

        private static Task EnterSyncState(GameSession session)
        {
            return session.StateMachine.MoveTo<SyncState>();
        }
    }
}
