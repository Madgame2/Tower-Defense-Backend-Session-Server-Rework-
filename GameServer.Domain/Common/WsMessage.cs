using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GameServer.Contracts.WsMessaging.Base
{
    public class WsMessage
    {
        public string? Action {  get; set; }
        public RawData Data { get; set; }
    }


    public readonly struct RawData
    {
        private readonly JsonElement _json;
        private readonly ReadOnlyMemory<byte> _binary; 
        private readonly string? _text;
        private readonly DataFormat _format;

        private enum DataFormat : byte { Empty, Json, Binary, Text }

        public RawData(JsonElement json)
        {
            _json = json;
            _format = DataFormat.Json;
            _binary = default;
            _text = null;
        }

        public RawData(ReadOnlyMemory<byte> binary)
        {
            _binary = binary;
            _format = DataFormat.Binary;
            _json = default;
            _text = null;
        }

        public RawData(string text)
        {
            _text = text;
            _format = DataFormat.Text;
            _json = default;
            _binary = default;
        }

        public T? Deserialize<T>()
        {
            return _format switch
            {
                DataFormat.Json => JsonSerializer.Deserialize<T>(_json.GetRawText()),
                DataFormat.Binary => MessagePackSerializer.Deserialize<T>(_binary),
                DataFormat.Text => typeof(T) == typeof(string) ? (T)(object)_text! : default,
                DataFormat.Empty => default,

                _ => throw new NotSupportedException("Unknown data format")
            };
        }
    }
}
