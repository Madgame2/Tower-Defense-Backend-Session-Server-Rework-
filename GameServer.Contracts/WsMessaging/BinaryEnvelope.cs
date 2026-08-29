using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.WsMessaging
{
    [MessagePackObject]
    public class BinaryEnvelope
    {
        // Порядок [Key(n)] определяет порядок полей в бинарном потоке
        [Key(0)] public string Action { get; set; }

        // nullable, так как для простого сообщения его нет
        [Key(1)] public Guid? RequestId { get; set; }

        // Сами данные (уже сериализованный DTO)
        [Key(2)] public byte[] DataBytes { get; set; }

        [IgnoreMember]
        public bool HasRequestId => RequestId.HasValue;
    }
}
