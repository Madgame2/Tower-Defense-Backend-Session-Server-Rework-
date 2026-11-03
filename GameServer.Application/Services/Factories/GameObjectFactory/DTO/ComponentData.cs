using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.DTO
{
    public sealed class ComponentData
    {
        public string Type { get; init; } = string.Empty;
        public XElement Metadata { get; init; } = new("Metadata");
        public XElement Data { get; init; } = new("Data");
    }
}
