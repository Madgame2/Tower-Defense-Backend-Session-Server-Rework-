using System;
using System.Collections.Generic;
using System.Text;

namespace EntityLib.Models
{
    public readonly struct Entity : IEquatable<Entity>
    {
        internal readonly int Id;
        internal readonly ushort Version;

        internal Entity(int id, ushort version)
        {
            Id = id;
            Version = version;
        }

        public bool Equals(Entity other) =>
            Id == other.Id &&
            Version == other.Version;

        public override bool Equals(object? obj) =>
            obj is Entity other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(Id, Version);

        public static bool operator ==(Entity left, Entity right) =>
            left.Equals(right);

        public static bool operator !=(Entity left, Entity right) =>
            !left.Equals(right);
    }
}
