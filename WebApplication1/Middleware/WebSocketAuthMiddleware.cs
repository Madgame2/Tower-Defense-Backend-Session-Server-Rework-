using GameServer.Application.Sessions;
using GameServer.Domain.Sessions;

namespace GameServer.Middleware
{
    public class WebSocketAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly IGameSessionService _gameSessions;
        public WebSocketAuthMiddleware(RequestDelegate next, IWebHostEnvironment env, IGameSessionService gameSessions)
        {
            _next = next;
            _env = env;
            _gameSessions = gameSessions;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            if (_env.IsDevelopment())
            {
                if (context.Request.Headers["EnterMode"] == "Development")
                {
                    string userId = Guid.NewGuid().ToString();
                    var sessionId = await _gameSessions.CreateSession();

                    var session = _gameSessions.Get(sessionId);
                    session.reservForPlayer(userId);

                    context.Request.Headers["UserId"] = userId;
                    context.Request.Headers["SessionId"] = sessionId.ToString();
                    await _next(context);
                    return;
                }
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("WebSocket connection rejected: unauthorized");
            return;
        }

    }
}
