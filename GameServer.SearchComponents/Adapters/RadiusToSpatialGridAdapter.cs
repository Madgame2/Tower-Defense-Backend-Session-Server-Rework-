using GameServer.Domain.SessionWorld.Services.IndicesService.Queries;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.Adapters
{
    public class RadiusToSpatialGridAdapter : QueryAdapter<RadiusOperation, SpatiialGridQuery>
    {
        public override SpatiialGridQuery Adapt(RadiusOperation query)
        {
            throw new NotImplementedException();
        }
    }
}
