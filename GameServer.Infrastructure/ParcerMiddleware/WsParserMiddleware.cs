using GameServer.Application.Messaging;
using GameServer.Contracts.WsMessaging.Base;
using GameServer.Services.WS.WSMiddleware.Model;
using MessagePack;
using System.Text.Json;
using GameServer.Contracts.WsMessaging;

namespace GameServer.Services.WS.WSMiddleware.Imp.ParcerMiddleware
{
    public class WsParserMiddleware : IMessageMiddleware
    {
        private static readonly JsonDocumentOptions _jsonDocOptions = new JsonDocumentOptions
        {
            AllowTrailingCommas = true 
        };

        public async Task<WsMessageContext> InvokeAsync(WsMessageContext context, MiddlewareDelegate next)
        {
            WsMessage? message = null;
            try
            {
                if (!context.Binary.IsEmpty && context.Binary.Length > 0)
                {
                    message = ParseBinary(context.Binary);
                }
                else if (!string.IsNullOrEmpty(context.Text))
                {
                    message = ParseText(context.Text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parsing error: {ex.Message}");

                message = new WsMessage
                {
                    Action = "RawText",
                    Data = new RawData(context.Text) 
                };
            }

            if (message != null)
            {
                context.Message = message;
            }
            context = await next(context);

            return context;
        }

        private WsMessage ParseBinary(ReadOnlyMemory<byte> data)
        {
            var raw = MessagePackSerializer.Deserialize<BinaryEnvelope>(data);

            var msg = raw.HasRequestId ? new WsRequest { RequestId = raw.RequestId.Value } : new WsMessage();
            msg.Action = raw.Action;
            msg.Data = new RawData(raw.DataBytes);

            return msg;
        }

        private WsMessage ParseText(string json)
        {
            using var doc = JsonDocument.Parse(json, _jsonDocOptions);
            var root = doc.RootElement;

            var msg = root.TryGetProperty("requestId", out _) ? new WsRequest() : new WsMessage();

            msg.Action = root.GetProperty("action").GetString();
            if (msg is WsRequest req) req.RequestId = root.GetProperty("requestId").GetGuid();

            if (root.TryGetProperty("data", out var dataProp))
            {
                msg.Data = new RawData(dataProp.Clone());
            }
            return msg;
        }
    }
}
