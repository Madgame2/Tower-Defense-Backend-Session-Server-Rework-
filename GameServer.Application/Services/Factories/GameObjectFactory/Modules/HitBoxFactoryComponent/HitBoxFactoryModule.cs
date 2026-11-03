using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model.Union;
using GameServer.Application.Services.Factories.GameObjectFactory.DTO;
using GameServer.Application.Services.Factories.GameObjectFactory.Modules.Base;
using GameServer.Domain.Core.GameObjects.Components;
using GameServer.Infrastructure.Serialization;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GameServer.Application.Services.Factories.GameObjectFactory.Modules.HitBoxFactoryComponent
{
    public class HitBoxFactoryModule : GameObjectComponentFactoryModule
    {
        public override string Component => "Hitbox";

        public override ComponentBase CreateByXML(ComponentData data)
        {
            var shapeTypeString = data.Metadata.Element("ShapeType")?.Value;

            if (string.IsNullOrWhiteSpace(shapeTypeString))
            {
                throw new FormatException(
                    "Hitbox metadata does not contain ShapeType.");
            }

            if (!Enum.TryParse<HitboxShapeType>(
                    shapeTypeString, true, out var shapeType)
                || !Enum.IsDefined(typeof(HitboxShapeType), shapeType))
            {
                throw new FormatException(
                    $"Invalid hitbox shape type: {shapeTypeString}.");
            }

            var hitboxData = shapeType switch
            {
                HitboxShapeType.Sphere =>
                    CreateHitbox<SphereHitboxDataXml, SphereHitboxData>(
                        data.Data,
                        xml => new SphereHitboxData
                        {
                            Center = new Vector3(
                                xml.Center.x,
                                xml.Center.y,
                                xml.Center.z),

                            Rotation = new Quaternion(
                                xml.Rotation.x,
                                xml.Rotation.y,
                                xml.Rotation.z,
                                xml.Rotation.w),

                            Radius = xml.Radius
                        },
                        shape => new HitboxShapeUnion { Sphere = shape }),

                HitboxShapeType.Capsule =>
                    CreateHitbox<CapsuleHitboxDataXml, CapsuleHitboxData>(
                        data.Data,
                        xml => new CapsuleHitboxData
                        {
                            Center = new Vector3(
                                xml.Center.x,
                                xml.Center.y,
                                xml.Center.z),

                            Rotation = new Quaternion(
                                xml.Rotation.x,
                                xml.Rotation.y,
                                xml.Rotation.z,
                                xml.Rotation.w),

                            Radius = xml.Radius,
                            Height = xml.Height
                        },
                        shape => new HitboxShapeUnion { Capsule = shape }),

                HitboxShapeType.Box =>
                    CreateHitbox<BoxHitboxDataXml, BoxHitboxData>(
                        data.Data,
                        xml => new BoxHitboxData
                        {
                            Center = new Vector3(
                                xml.Center.x,
                                xml.Center.y,
                                xml.Center.z),

                            Rotation = new Quaternion(
                                xml.Rotation.x,
                                xml.Rotation.y,
                                xml.Rotation.z,
                                xml.Rotation.w),

                            Size = new Vector3(
                                xml.Size.x,
                                xml.Size.y,
                                xml.Size.z)
                        },
                        shape => new HitboxShapeUnion { Box = shape }),

                _ => throw new NotSupportedException(
                    $"Unsupported hitbox shape: {shapeType}")
            };

            return new HitBoxComponent
            {
                Data = new HitboxData
                {
                    ShapeType = shapeType,
                    Shape = hitboxData
                }
            };
        }

        private static HitboxShapeUnion CreateHitbox<TXml, T>(
            XElement data,
            Func<TXml, T> convert,
            Func<T, HitboxShapeUnion> createUnion)
        {
            var serializer = new XmlSerializer(
                typeof(TXml),
                new XmlRootAttribute("Data"));

            using var reader = data.CreateReader();

            var xmlData = (TXml?)serializer.Deserialize(reader)
                ?? throw new FormatException(
                    $"Failed to deserialize hitbox data of type {typeof(TXml).Name}.");

            return createUnion(convert(xmlData));
        }
    }
}
