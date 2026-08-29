using GameServer.Application.Messaging;
using GameServer.Services.WS.WSMiddleware.Model;

namespace GameServer.Services.WS.WSMiddleware.Imp.LoggingMiddleware
{
    public class LoggingMiddleware : IMessageMiddleware
    {
        public async Task<WsMessageContext> InvokeAsync(WsMessageContext context, MiddlewareDelegate next)
        {
            Console.WriteLine($"FROM {context.ConnectionContext.PlayerId}, sessionId: {context.ConnectionContext.SessionId}");
            Console.WriteLine($"IN: {context.Text ?? "[binary]"}");

            context = await next(context);

            Console.WriteLine($"OUT");

            return context;
        }
    }
}
