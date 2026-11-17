using GameServer.Application.Services.Factories.GameObjectFactory.DTO;
using GameServer.Application.Services.Factories.GameObjectFactory.Modules.Base;
using GameServer.Application.Services.Factories.GameObjectFactory.Modules.UpperBodyFactoryComponent.DTO;
using GameServer.Domain.Core.GameObjects.Components;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterDefenitions.UpperBody;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Modules.UpperBodyFactoryComponent
{

    public class UpperBodyFactoryModule : GameObjectComponentFactoryModule
    {
        public override string Component => "UpperBodyComponent";

        public override ComponentBase CreateByXML(ComponentData data)
        {
            var serializer = new XmlSerializer(
                typeof(UpperBodyDataXml),
                new XmlRootAttribute("Data"));

            using var reader = data.Data.CreateReader();

            var xmlData = (UpperBodyDataXml?)serializer.Deserialize(reader)
                ?? throw new FormatException(
                    "Failed to deserialize UpperBody data.");

            return new UpperBodyComponent(
                new Vector3(
                    xmlData.PivotLocalPosition.x,
                    xmlData.PivotLocalPosition.y,
                    xmlData.PivotLocalPosition.z),

                new Vector3(
                    xmlData.AimOriginOffsetFromPivot.x,
                    xmlData.AimOriginOffsetFromPivot.y,
                    xmlData.AimOriginOffsetFromPivot.z),

                new Vector3(
                    xmlData.AimForwardLocal.x,
                    xmlData.AimForwardLocal.y,
                    xmlData.AimForwardLocal.z)
            );
        }
    }
}
