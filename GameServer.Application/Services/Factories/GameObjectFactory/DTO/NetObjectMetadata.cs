using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Services.Factories.GameObjectFactory.DTO
{
    public struct NetObjectMetadata
    {
        public string ObjectType { get; init; } = string.Empty;

        public string ObjectName { get; init; } = string.Empty;

        public int Version { get; init; }

        public NetObjectMetadata()
        {

        }

        public NetObjectMetadata(string objectType, string objectNmae, int version) {
            ObjectType = objectType;
            ObjectName = objectNmae;
            Version = version;
        }
    }
}
