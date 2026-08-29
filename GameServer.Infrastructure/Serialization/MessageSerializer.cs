using GameServer.Application.Interfaces;
using GameServer.Contracts.MessageFormats;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GameServer.Infrastructure.Serialization
{
    public class MessageSerializer: IMessageSerializer
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        };

        private readonly MessagePackSerializerOptions _msgPackOptions = MessagePackSerializerOptions.Standard
            .WithCompression(MessagePackCompression.Lz4BlockArray);

        public (byte[] Buffer, int Length) SerializePooled<T>(T message, MessageFormat format)
        {
            var writer = new PooledBufferWriter(initialCapacity: 1024);

            switch (format)
            {
                case MessageFormat.Json:
                    using (var jsonWriter = new Utf8JsonWriter(writer))
                    {
                        JsonSerializer.Serialize(jsonWriter, message, _jsonOptions);
                    }
                    break;

                case MessageFormat.Binary:
                    MessagePackSerializer.Serialize(writer, message, _msgPackOptions);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }

            return (writer.DetachBuffer(), writer.WrittenCount);
        }

        public byte[] SerializeToArray<T>(T message, MessageFormat format)
        {
            switch (format)
            {
                case MessageFormat.Json:
                    return JsonSerializer.SerializeToUtf8Bytes(message, _jsonOptions);

                case MessageFormat.Binary:
                    throw new NotImplementedException("Бинарный формат пока не подключен");

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}
