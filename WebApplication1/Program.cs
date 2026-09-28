using GameServer.Api.WebSockets;
using GameServer.Application.Interfaces;
using GameServer.Application.Meta;
using GameServer.Application.Session.Ressources;
using GameServer.Application.Sessions;
using GameServer.Application.Sessions.imp;
using GameServer.Application.Sessions.Repository;
using GameServer.Application.Sessions.States;
using GameServer.Domain.Common.Interfaces;
using GameServer.Domain.Interfaces;
using GameServer.Domain.Sessions;
using GameServer.Domain.Sessions.Repository;
using GameServer.Domain.Sessions.StateMachine.Factory;
using GameServer.Domain.Sessions.StateMachine.StatesGraph;
using GameServer.Domain.SessionWorld.Factory;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Factories;
using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Factory;
using GameServer.Domain.SessionWorld.Graphs.LandscapeGraph.Interfaces;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph.Factory;
using GameServer.Domain.SessionWorld.Graphs.TreeGraph.Factory.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Meta.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery;
using GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces;
using GameServer.Domain.UDP.Interfaces;
using GameServer.Endpoints;
using GameServer.GameLoop.Core.Simultaion.Factory;
using GameServer.GameLoop.Core.Simultaion.Factory.Interfaces;
using GameServer.Infrastructure.Connection;
using GameServer.Infrastructure.Factory.WSContext;
using GameServer.Infrastructure.Networking;
using GameServer.Infrastructure.Networking.SyncService;
using GameServer.Infrastructure.Serialization;
using GameServer.Infrastructure.Services.StaticData;
using GameServer.Infrastructure.Sessions.Repository;
using GameServer.Infrastructure.UDP.Core;
using GameServer.Infrastructure.WSRouter;
using GameServer.Middleware;
using GameServer.Services.WS.WSMiddleware;
using GameServer.Services.WS.WSMiddleware.Imp.LoggingMiddleware;
using GameServer.Services.WS.WSMiddleware.Imp.ParcerMiddleware;
using GameServer.Services.WS.WSMiddleware.Model;
using Microsoft.Extensions.Options;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);
var port = builder.Configuration.GetValue<int>("Server:Port");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

builder.Services.Configure<WorldSettings>(
    builder.Configuration.GetSection("GameWorld")
);

builder.Services.AddTransient<IChunksSettings>(sp =>

    sp.GetRequiredService<IOptions<WorldSettings>>().Value
);


builder.Services.AddControllers();

builder.Services.AddSingleton<IStaticDataService, StaticDataService>(provider=> {

    var config = provider.GetRequiredService<IConfiguration>();

    var pathFromConfig = config.GetValue<string>("StaticDataPath") ?? "./Resources";

    string finalPath = Path.IsPathFullyQualified(pathFromConfig)
        ? pathFromConfig
        : Path.Combine(AppContext.BaseDirectory, pathFromConfig);

    return new StaticDataService(finalPath);
});
builder.Services.AddSingleton<IConnectedClientsStorage, ConnectionStorage>();
builder.Services.AddSingleton<StateFactory>();
builder.Services.AddSingleton<GameSessionFactory>();
builder.Services.AddTransient<ILandscapeGraphFactory, LandscapeGraphFactory>();
builder.Services.AddTransient<IChunkGeneratorFactory, ChunkGeneratorFactory>();
builder.Services.AddSingleton<StateGraph>(provider =>
{
    var graph = new StateGraph();
    graph.InitGraph(typeof(SessionCreatedState).Assembly);

    return graph;
});

builder.Services.AddTransient<IWsRouter, WsRouter>();
builder.Services.AddSingleton<IWsContextFactory, WsContextFactory>();

builder.Services.AddSingleton<MessagePipeline>();
builder.Services.AddScoped<SessionSocketHandler>();

builder.Services.AddSingleton<IMessageMiddleware, LoggingMiddleware>();
builder.Services.AddSingleton<IMessageMiddleware, WsParserMiddleware>();

builder.Services.AddSingleton<ISessionRegistry, SessionRegistry>();
builder.Services.AddSingleton<IGameSessionRepository, InMemmoryGameSessionRepository>();
builder.Services.AddSingleton<IGameSessionService, GameSessionService>();
builder.Services.AddSingleton<ISessionBroadcaster, SessionBroadcaster>();
builder.Services.AddTransient<IMessageSerializer, MessageSerializer>();
builder.Services.AddSingleton<ISyncService, SyncService>();

builder.Services.AddTransient<IBiomGraphFactory, BiomGraphFactory>();

builder.Services.AddTransient<ITreeGraphFactory, TreeGraphFactory>();

builder.Services.AddTransient<ISimulationFactory, SimulationFactory>();  

builder.Services.AddSingleton<IWorldQueryService, WorldQueryService>();

builder.Services.AddSingleton<ISessionStaticResources, SessionStaticResources>();


builder.Services.AddSingleton<UdpServerTransport>();
builder.Services.AddSingleton<IUdpSender>(sp => sp.GetRequiredService<UdpServerTransport>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<UdpServerTransport>());

var app = builder.Build();

app.UseWebSockets();
app.UseMiddleware<WebSocketAuthMiddleware>();

app.MapSocketEndpoints();

app.Run();