using GameServer.Application.Services.Factories.GameObjectFactory.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Modules.UpperBodyFactoryComponent.DTO
{
    [Serializable]
    [XmlRoot("Data")]
    public struct UpperBodyDataXml
    {
        public Vector3Xml PivotLocalPosition { get; set; }

        public Vector3Xml AimOriginOffsetFromPivot { get; set; }

        public Vector3Xml AimForwardLocal { get; set; }
    }
}
