using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.IndicesService.Queries;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.IndicesService.IndexesImplement
{
    public class SpatialGridIndex<T> : IIndex<T, SpatiialGridQuery>
    {
        private readonly Func<T, Vector2> _keySelector;
        private readonly Dictionary<GridKey, List<T>> _cellsStorage = new();

        public SpatialGridIndex(Func<T, Vector2> keySelector)
        {
            _keySelector = keySelector;
        }

        public void Add(T item)
        {
            Vector2 position = _keySelector(item);
            GridKey normalizedPosition = GetKey(position);

            if (!_cellsStorage.TryGetValue(normalizedPosition, out var cell))
            {
                cell = new();
            }
            cell.Add(item);
        }

        public void Dispose()
        {
            foreach (var cell in _cellsStorage.Values)
            {
                cell.Clear();
            }
            _cellsStorage.Clear();
        }

        public void Get(
            SpatiialGridQuery query,
            List<T> result)
        {
            GridKey center = GetKey(query.position);

            int radius = query.radius;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int cellRadius = Math.Max(
                        Math.Abs(x),
                        Math.Abs(y)
                    );

                    GridKey key = new GridKey(
                        center.X + x,
                        center.Y + y
                    );

                    if (!_cellsStorage.TryGetValue(key, out var cell))
                        continue;

                    result.AddRange(cell);
                }
            }
        }

        public void Remove(T item)
        {
            Vector2 position = _keySelector(item);
            GridKey normalizedPosition = GetKey(position);

            if (_cellsStorage.TryGetValue(normalizedPosition, out var cell))
            {
                cell?.Remove(item);
            }
        }

        private GridKey GetKey(Vector2 position)
        {
            return new GridKey(
                (int)MathF.Floor(position.X),
                (int)MathF.Floor(position.Y)
            );
        }

        public readonly struct GridKey : IEquatable<GridKey>
        {
            public readonly int X;
            public readonly int Y;

            public GridKey(int x, int y)
            {
                X = x;
                Y = y;
            }

            public bool Equals(GridKey other)
                => X == other.X && Y == other.Y;

            public override bool Equals(object? obj)
                => obj is GridKey other && Equals(other);

            public override int GetHashCode()
                => HashCode.Combine(X, Y);
        }
    }
}
