

using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Adapters.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators;
using GameServer.SearchComponents.Data;
using System.Numerics;

namespace GameServer.SearchComponents.Adapters.FullScan
{
    public class RadiusFullScanAdapter : FullScanAdapter<RadiusOperation, PositionQueryData>
    {
        public override ScanAdapterDelegate<T1, PositionQueryData> CreateExecutor<T1>()
        {
            return (checkedPosition, operation, context) =>
            {
                var radiusOperation = (RadiusOperation)operation;

                var center = radiusOperation.Center;

                return Vector3.Distance(center, checkedPosition.Position) <= radiusOperation.Radius;
            };
        }
    }
}
