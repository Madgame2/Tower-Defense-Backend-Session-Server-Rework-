using GameServer.Application.Messaging;
using GameServer.Services.WS.WSMiddleware.Model;

namespace GameServer.Services.WS.WSMiddleware
{
    public class MessagePipeline
    {
        private readonly List<IMessageMiddleware> _middlewares;

        public MessagePipeline(IEnumerable<IMessageMiddleware> middlewares)
        {
            _middlewares = middlewares.ToList();
        }

        public Task<WsMessageContext> Execute(WsMessageContext context)
        {
            var index = 0;

            Task<WsMessageContext> Next(WsMessageContext currentContext)
            {
                if (index < _middlewares.Count)
                {
                    var mw = _middlewares[index++];

                    return mw.InvokeAsync(currentContext, Next);
                }

                return Task.FromResult(currentContext);
            }

            return Next(context);
        }
    }
}
