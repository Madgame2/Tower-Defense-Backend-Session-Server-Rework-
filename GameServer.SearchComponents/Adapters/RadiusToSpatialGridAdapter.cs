using GameServer.Domain.SessionWorld.Services.IndicesService.Interfaces;
using GameServer.Domain.SessionWorld.Services.IndicesService.Queries;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.SearchComponents.Adapters
{
    public class RadiusToSpatialGridAdapter : QueryAdapter<RadiusOperation, SpatiialGridQuery>
    {
        public override SpatiialGridQuery Adapt(RadiusOperation query)
        {
            throw new NotImplementedException();
        }

        public override IndexExecutorDelegate<T> CreateExecutor<T>(IIndex<T> index)
        {
            var typedIndex = (IIndex<T, SpatiialGridQuery>)index;

            return (index, operation, context, buffer) =>
            {
                var radiusOperation = (RadiusOperation)operation;

                var center = radiusOperation.Center;

                SpatiialGridQuery query = new SpatiialGridQuery {
                    position = new Vector2(center.X, center.Z),
                    radius = (int)MathF.Ceiling(radiusOperation.Radius)
                };

                typedIndex.Get(query, buffer);
            };
        }
    }
}
