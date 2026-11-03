using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.DTO
{
    [XmlRoot("NetObject")]
    public struct NetObjectData
    {
        public NetObjectMetadata Metadata { get; set; } = new NetObjectMetadata();

        [XmlArray("Components")]
        [XmlArrayItem("Component")]
        public List<ComponentData> Components { get; init; } = new();

        public NetObjectData() { }
    }
}
