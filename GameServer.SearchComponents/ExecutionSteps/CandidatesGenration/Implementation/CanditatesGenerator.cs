using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Interfaces;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.DataSourceModules.Base;
using GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Implementation
{
    public class CanditatesGenerator : IQueryPlanerCanditatesGenerator
    {
        private readonly IEnumerable<IPossibilitySoureseModuel> _dataSourceModules;

        public CanditatesGenerator(IEnumerable<IPossibilitySoureseModuel> dataSoureseModuels)
        {
            _dataSourceModules = dataSoureseModuels.ToList();
        }

        ICandidatesMatrix IQueryPlanerCanditatesGenerator.Generate<T>(IReadOnlyList<IQueryOperation> operations, QueryContext<T> queryContext)
        {
            var resultMatrix = new CandidatesMatrix();

            foreach (var operation in operations)
            {
                foreach (var module in _dataSourceModules)
                {
                    var hasPossibilities = module.TryGetPossibilities<T>(operation, queryContext, out var possibilities);

                    if (hasPossibilities)
                    {
                        resultMatrix.Add(operation, possibilities);
                    }
                }
            }

            return resultMatrix;
        }
    }
}
