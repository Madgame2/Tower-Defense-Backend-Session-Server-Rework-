using GameServer.Domain.SessionWorld.Graphs.BiomGraph.Meta.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Model
{
    public class Chank
    {
        public Vector2 Position { get; }
        public int Size { get; }
        public Vector2 Pivot { get; } 

        public float[] HeightMap { get; }
        public BiomeType[] BiomeMap { get; }

        public List<StaticTreeData> Trees { get; }

        public Chank(Vector2 offset, int size, Vector2 pivot)
        {
            Position = offset;
            Size = size;
            Pivot = pivot;

            int vertexCount = (size + 1) * (size + 1);

            HeightMap = new float[vertexCount];
            BiomeMap = new BiomeType[size * size];
        }

        public void AddTree(Vector2 localPos)
        {
            Trees.Add(new StaticTreeData
            {
                LocalPosition = localPos,
            });
        }

        public void SetlandscapeHeight(int x, int y, float value)
        {
            int vertexSize = Size + 1;
            HeightMap[x + y * vertexSize] = value;
        }

        public void SetBiomeCell(int x, int y, BiomeType type)
        {
            BiomeMap[x + y * Size] = type;
        }

        public float GetLandscapeHeight(int x, int z)
        {
            int vertexSize = Size + 1;

            x = Math.Clamp(x, 0, vertexSize - 1);
            z = Math.Clamp(z, 0, vertexSize - 1);

            return HeightMap[x + z * vertexSize];
        }
    }
}
