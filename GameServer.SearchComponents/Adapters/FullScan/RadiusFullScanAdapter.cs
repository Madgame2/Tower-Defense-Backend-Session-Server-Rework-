using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.SearchComponents.Adapters.FullScan
{
    public class RadiusFullScanAdapter<T>
    : FullScanAdapter<RadiusOperation, T>
    {
        private readonly PositionGetter<T> _getPosition;

        public RadiusFullScanAdapter(PositionGetter<T> getPosition)
        {
            _getPosition = getPosition;
        }

        public override FullScanExecutorDelegate<T> CreateExecutor()
        {
            var getPosition = _getPosition;

            return (operation, context, source, buffer) =>
            {
                var radiusOperation = (RadiusOperation)operation;

                Vector3 center = radiusOperation.Center;
                float radiusSquared =
                    radiusOperation.Radius * radiusOperation.Radius;

                for (int i = 0; i < source.Length; i++)
                {
                    ref readonly T item = ref source[i];

                    Vector3 position = getPosition(in item);

                    if (Vector3.DistanceSquared(position, center) <= radiusSquared)
                    {
                        buffer.Add(item);
                    }
                }
            };
        }
    }
}
