using GameServer.Application.Services.Factories.GameObjectFactory.DTO;
using GameServer.Application.Services.Factories.GameObjectFactory.Modules.Base;
using GameServer.Application.Services.Factories.GameObjectFactory.Modules.SpawnAnchorFactoryComponent.DTO;
using GameServer.Domain.Core.GameObjects.Components;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.SpawnPointAnchor;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Modules.SpawnAnchorFactoryComponent
{
    public class SpawnAnchorFactiryModule : GameObjectComponentFactoryModule
    {
        public override string Component => "SpawnPointAnchor";

        public override ComponentBase CreateByXML(ComponentData data)
        {
            var serializer = new XmlSerializer(
                typeof(SpawnPointAnchorDataXml),
                new XmlRootAttribute("Data"));

            using var reader = data.Data.CreateReader();

            var xmlData = (SpawnPointAnchorDataXml?)serializer.Deserialize(reader)
                ?? throw new FormatException(
                    "Failed to deserialize SpawnPointAnchor data.");

            return new SpawnPointAnchorComponent
            {
                AnchorName = xmlData.AnchorName,


                SpawnPointLocal = new Vector3(
                    xmlData.SpawnPoint.X,
                    xmlData.SpawnPoint.Y,
                    xmlData.SpawnPoint.Z),

                PivotPointLocal = new Vector3(
                    xmlData.PivotPoint.X,
                    xmlData.PivotPoint.Y,
                    xmlData.PivotPoint.Z)
            };
        }
    }
}
