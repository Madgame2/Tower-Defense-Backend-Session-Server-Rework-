using GameServer.Api.WebSockets;
using GameServer.Services.WS.WSMiddleware;
using GameServer.Services.WS.WSMiddleware.Model;

namespace GameServer.Endpoints
{
    public static class WebSocketEndpoints
    {
        public static void MapSocketEndpoints(this WebApplication app)
        {
            app.Map("/ws", HandleConnection);
        }

        private static async Task HandleConnection(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();

            var handler = context.RequestServices.GetRequiredService<SessionSocketHandler>();

            await handler.Handle(socket, context);
        }
    }
}
