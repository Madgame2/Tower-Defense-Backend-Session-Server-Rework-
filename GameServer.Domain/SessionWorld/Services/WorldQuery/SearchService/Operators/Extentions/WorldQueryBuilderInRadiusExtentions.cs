using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Builder;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Extentions
{
    public static class WorldQueryBuilderInRadiusExtentions
    {
        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, string centerArg, string radiosArg)
        {
            var ceneterParam = new NamedParameter<Vector3>(centerArg);
            var radiosParam = new NamedParameter<float>(radiosArg);

            var newOperation = new RadiusOperation(ceneterParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, string centerArg, float radios)
        {
            var ceneterParam = new NamedParameter<Vector3>(centerArg);
            var radiosParam = new ConstParameter<float>(radios);

            var newOperation = new RadiusOperation(ceneterParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, Vector3 centerArg, string radiosArg)
        {
            var ceneterParam = new ConstParameter<Vector3>(centerArg);
            var radiosParam = new NamedParameter<float>(radiosArg);

            var newOperation = new RadiusOperation(ceneterParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, Vector3 centerArg, float radiosArg)
        {
            var ceneterParam = new ConstParameter<Vector3>(centerArg);
            var radiosParam = new ConstParameter<float>(radiosArg);

            var newOperation = new RadiusOperation(ceneterParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, IQueryParameter<Vector3> centerParam, float radiosArg)
        {
            var radiosParam = new ConstParameter<float>(radiosArg);

            var newOperation = new RadiusOperation(centerParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, IQueryParameter<Vector3> centerParam, string radiosArg)
        {
            var radiosParam = new NamedParameter<float>(radiosArg);

            var newOperation = new RadiusOperation(centerParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, string centerArg, IQueryParameter<float> radiosParam)
        {
            var centerParam = new NamedParameter<Vector3>(centerArg);

            var newOperation = new RadiusOperation(centerParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, Vector3 centerArg, IQueryParameter<float> radiosParam)
        {
            var centerParam = new ConstParameter<Vector3>(centerArg);

            var newOperation = new RadiusOperation(centerParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }

        public static WorldQueryBuilder<T> InRadius<T>(this WorldQueryBuilder<T> queryBuilder, IQueryParameter<Vector3> centerParam, IQueryParameter<float> radiosParam)
        {
            var newOperation = new RadiusOperation(centerParam, radiosParam);

            queryBuilder.Operations.Add(newOperation);

            return queryBuilder;
        }
    }
}
