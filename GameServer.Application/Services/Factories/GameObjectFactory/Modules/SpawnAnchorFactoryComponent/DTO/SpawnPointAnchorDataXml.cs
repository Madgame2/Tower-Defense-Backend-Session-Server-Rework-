using GameServer.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Modules.SpawnAnchorFactoryComponent.DTO
{
    [Serializable]
    [XmlRoot("Data")]
    public struct SpawnPointAnchorDataXml
    {
        public string AnchorName { get; set; } = string.Empty;

        public Vector3Xml SpawnPoint { get; set; }
        public Vector3Xml PivotPoint { get; set; }

        public SpawnPointAnchorDataXml()
        {

        }
    }
}
