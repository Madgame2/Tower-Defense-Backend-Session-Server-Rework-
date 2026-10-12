using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Operators.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.CandidatesGenration.Model
{
    public partial class CandidatesMatrix : ICandidatesMatrix
    {
        private HashSet<IQueryOperation> _operations = new HashSet<IQueryOperation>();

        private Dictionary<IQueryOperation, List<PhysicalCandidate>> _operationsMatrix = new();


        public HashSet<IQueryOperation> Operations => _operations;
        public Dictionary<IQueryOperation, List<PhysicalCandidate>> OperationsMatrix => _operationsMatrix;

        public void Add(IQueryOperation operation, List<PhysicalCandidate> possibilities)
        {
            _operations.Add(operation);

            if (!_operationsMatrix.TryGetValue(operation, out var list))
            {
                list = new List<PhysicalCandidate>();
                _operationsMatrix[operation] = list;
            }

            list.AddRange(possibilities);
        }
    }
}
