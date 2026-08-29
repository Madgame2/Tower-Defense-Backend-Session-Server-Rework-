using GameServer.Application.Interfaces;
using GameServer.Application.Messaging;
using GameServer.Application.Model;
using GameServer.Application.Sessions;
using GameServer.Application.Sessions.imp;
using GameServer.Contracts.MessageFormats;
using GameServer.Contracts.ServerMessages;
using GameServer.Services.WS.WSMiddleware;
using GameServer.WebSockets;
using System.Buffers.Binary;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;

namespace GameServer.Api.WebSockets;

public class SessionSocketHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IGameSessionService _gameSessionService;
    private readonly IConnectedClientsStorage _connectidClientStorage;
    private readonly ISessionBroadcaster sessionBroadcaster;

    public SessionSocketHandler(IServiceProvider serviceProvider,
        IGameSessionService gameSessionService,
        IConnectedClientsStorage connectidClientStorage,
        ISessionBroadcaster sessionBroadcaster)
    {
        _serviceProvider = serviceProvider;
        _gameSessionService = gameSessionService;
        _connectidClientStorage = connectidClientStorage;
        this.sessionBroadcaster = sessionBroadcaster;
    }

    public async Task Handle(WebSocket socket, HttpContext context)
    {
        var token = context.RequestAborted;

        if (!TryGetPlayerAndSession(context, out var playerId, out var sessionId))
        {
            await CloseSocket(socket, WebSocketCloseStatus.PolicyViolation, "Invalid auth data", token);
            return;
        }

        var session = _gameSessionService.Get(sessionId);

        if (session == null)
        {
            await CloseSocket(socket,
                WebSocketCloseStatus.PolicyViolation,
                "Session not found",
                token);

            return;
        }

        var connection = ActivatorUtilities.CreateInstance<ClientConnection>(_serviceProvider);
        var udpToken = GenerateSecureUdpToken();

        connection.Init(playerId, udpToken, session, socket, token);

        _connectidClientStorage.Add(connection);

        connection.StartReadingLoop();
        connection.StartWritingLoop();

        var attachResult = await _gameSessionService.Attach(playerId, sessionId);

        if (!attachResult.Success)
        {
            await _connectidClientStorage.Remove(connection.UserId);

            await CloseSocket(socket,
                WebSocketCloseStatus.PolicyViolation,
                attachResult.ErrorMessage,
                token);

            return;
        }

        await connection.Completion;
    }


    private uint GenerateSecureUdpToken()
    {
        Span<byte> buffer = stackalloc byte[4];
        RandomNumberGenerator.Fill(buffer);
        return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    private async Task CloseSocket(
        WebSocket socket,
        WebSocketCloseStatus status,
        string description,
        CancellationToken token)
    {
        if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
        {
            await socket.CloseAsync(status, description, token);
        }
    }

    private bool TryGetPlayerAndSession(
        HttpContext context,
        out string playerId,
        out Guid sessionId)
    {
        playerId = null;
        sessionId = Guid.Empty;

        var playerIdHeader = context.Request.Headers["UserId"].FirstOrDefault();
        var sessionIdHeader = context.Request.Headers["SessionId"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(playerIdHeader))
            return false;

        if (!Guid.TryParse(sessionIdHeader, out sessionId))
            return false;

        playerId = playerIdHeader;
        return true;
    }
}