using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators
{
    public sealed class RadiusOperation : IQueryOperation
    {
        private IQueryParameter<Vector3> _center;
        private IQueryParameter<float> _radius;

        public RadiusOperation(IQueryParameter<Vector3> center, IQueryParameter<float> radius)
        {
            _center = center;
            _radius = radius;
        }

        public IEnumerable<IQueryParameter> GetParameters()
        {
            return new IQueryParameter[] { _center, _radius };
        }
    }
}
