using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Contracts.Common
{
    [Serializable]
    public struct Vector3Xml
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
