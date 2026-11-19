using EntityLib.GameRoomExtentions;
using GameServer.Application.Resources.PrefabsRegesters;
using GameServer.Application.Services.LightEnityesSpawner;
using GameServer.Application.Services.StaticResources;
using GameServer.Application.Session.Services.ObjectRegister;
using GameServer.Application.Session.Services.ObjectRegister.Handlers;
using GameServer.Application.Session.Services.ObjectRegister.Midleware;
using GameServer.Application.Session.Services.ObjectRegister.Midleware.Configs;
using GameServer.Application.Session.Services.ObjectRegister.Midleware.Meta;
using GameServer.Application.Session.Services.ReplicationService.Registry;
using GameServer.Application.Session.Services.Repositories;
using GameServer.Domain.ColliderSystem.Enum;
using GameServer.Domain.Player;
using GameServer.Domain.Services.StaticData;
using GameServer.Domain.Sessions.Events;
using GameServer.Domain.Sessions.Events.Stateshandler;
using GameServer.Domain.Sessions.StateMachine;
using GameServer.Domain.Sessions.StateMachine.StatesGraph.Attributes;
using GameServer.Domain.SessionWorld;
using GameServer.Domain.SessionWorld.ChunksService.ChunkStorage;
using GameServer.Domain.SessionWorld.Generators;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph.Factory.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces.LightEntitiesSpawnService;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.PlayerStorages;
using GameServer.Domain.SessionWorld.PlayerStorages.Interfaces;
using GameServer.Domain.SessionWorld.Routers;
using GameServer.Domain.SessionWorld.Services.IndicesService.IndexesImplement;
using GameServer.Domain.SessionWorld.Services.IndicesService.Storages;
using GameServer.Domain.SessionWorld.Services.ObjectRegister;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Handlers;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery;
using GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace GameServer.Application.Sessions.States
{
    [Transition(typeof(SyncState))]
    public class GenerateWorldState : BaseState
    {
        private readonly IChunksSettings _chunksSettings;
        private readonly IQueryExecutor _worldQueryService;
        private readonly IGameObjectSpawner _gameObjectSpawner;
        private readonly ILightEntitySpawner _lightEntitySpawner;
        private readonly IQueryPlanner _queryPlanner;
        private readonly IStaticResourceLoader _staticResourceLoader;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISessionStaticResources _sessionStaticResources;
        private IChunkGeneratorFactory _chankGeneratorFactory;
        private ILandscapeGraphFactory _landscapeGraphFactory;
        private IBiomGraphFactory _biomGraphFactory;
        private ITreeGraphFactory _treeGraphFactory;

        public GenerateWorldState(IChunksSettings chunksSettings,
            IQueryExecutor worldQueryService,
            IChunkGeneratorFactory chankFactory,
            ILandscapeGraphFactory landscapeGraphFactory,
            IBiomGraphFactory biomGraphFactory,
            ITreeGraphFactory treeGraphFactory,
            IQueryPlanner queryPlanner,
            IStaticResourceLoader statickResourceLoader,
            ILoggerFactory loggerFactory,
            IGameObjectSpawner gameObjectSpawner,
            ILightEntitySpawner lightEntitySpawner,
            ISessionStaticResources sessionStaticResources)
        {
            _worldQueryService = worldQueryService;
            _chunksSettings = chunksSettings;
            _chankGeneratorFactory = chankFactory;
            _landscapeGraphFactory = landscapeGraphFactory;
            _biomGraphFactory = biomGraphFactory;
            _treeGraphFactory = treeGraphFactory;
            _queryPlanner = queryPlanner;
            _staticResourceLoader = statickResourceLoader;
            _loggerFactory = loggerFactory;
            _gameObjectSpawner = gameObjectSpawner;
            _lightEntitySpawner = lightEntitySpawner;
            _sessionStaticResources = sessionStaticResources;
        }

        public override void Configure(StateEventHandler handlers)
        {
            base.Configure(handlers);

            handlers.Register<PlayerConnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player join {evt.UserId}");
            });

            handlers.Register<PlayerDisconnected>(async (ctx, evt) =>
            {
                Console.WriteLine($"Player leave {evt.UserId}");
            });
        }


        public override async Task OnEnter(GameSession ctx)
        {
            Console.WriteLine("ON GenerateWorld");

            var middlewareDescription = GameRoomMiddlewareConfig.Create();

            var router = new GeneralMathcRouter();
            var worldQueryService = new WorldQueryService(_queryPlanner, _worldQueryService, _sessionStaticResources);
            var replicationRegister = new ReplicationRegistry();
            var chunkStorage = new InMemmoryChunkStorage();
            var playersSorage = new InMemmoryPlayerStorage();
            var indexStorage = new InMemmoryIndexStorage();
            var hitBoxRepository = new HitBoxRepository();


            var services = new RoomServices()
                .Add(replicationRegister)
                .Add(chunkStorage)
                .Add(playersSorage)
                .Add(hitBoxRepository)
                .Add(indexStorage);

            var pipeline = middlewareDescription.Build(services);

            var objectRegister = new InMemmoryObjectRegister(pipeline);
            var deffoutlObjectRegester = new DefaultObjectRegister();
            deffoutlObjectRegester.BuildContext();



            IObjectHandler[] handlers =
            {
                new ChunkRegisterHandler(chunkStorage, indexStorage),
                new PlayersRegisterHandler(playersSorage, replicationRegister),
                new HitBoxRegesterHandler(hitBoxRepository)
            };

            var newWorldObject = new GameRoom(_chunksSettings,
                router,
                _worldQueryService,
                _queryPlanner, 
                _gameObjectSpawner,
                _lightEntitySpawner,
                worldQueryService,
                objectRegister,
                deffoutlObjectRegester,
                replicationRegister,
                playersSorage,chunkStorage, indexStorage, handlers);
            
            worldQueryService.GameRoom = newWorldObject;

            var treesIndex = new SpatialGridIndex<StaticTreeData>((treeData) => { return treeData.LocalPosition; });
            newWorldObject.AddIndex(treesIndex);

            CreateWorldGenerationRules(newWorldObject);
            //await SpawnPlayers(ctx.OnlinePlayersIDs, newWorldObject);

            ctx.SetRoom(newWorldObject);

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

        private static Task EnterSyncState(GameSession session)
        {
            return session.StateMachine.MoveTo<SyncState>();
        }
    }
}
