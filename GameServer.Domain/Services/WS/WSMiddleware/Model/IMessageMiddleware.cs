using GameServer.Application.Messaging;

namespace GameServer.Services.WS.WSMiddleware.Model
{
    public delegate Task<WsMessageContext> MiddlewareDelegate(WsMessageContext context);
    public interface IMessageMiddleware
    {
        Task<WsMessageContext> InvokeAsync(
            WsMessageContext context,
            MiddlewareDelegate next);
    }
}
