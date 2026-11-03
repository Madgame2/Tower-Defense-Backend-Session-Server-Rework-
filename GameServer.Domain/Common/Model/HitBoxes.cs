using GameServer.Application.Services.Factories.GameObjectFactory.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Infrastructure.Serialization
{
    public struct SphereHitboxDataXml
    {
        public Vector3Xml Center { get; set; }
        public QuaternionXml Rotation { get; set; }
        public float Radius { get; set; }
    }

    public struct CapsuleHitboxDataXml
    {
        public Vector3Xml Center { get; set; }
        public QuaternionXml Rotation { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }
    }

    public struct BoxHitboxDataXml
    {
        public Vector3Xml Center { get; set; }
        public QuaternionXml Rotation { get; set; }
        public Vector3Xml Size { get; set; }
    }
}
