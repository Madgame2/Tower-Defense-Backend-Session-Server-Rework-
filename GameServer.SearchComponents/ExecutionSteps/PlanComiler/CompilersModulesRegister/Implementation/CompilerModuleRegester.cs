using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules.Base;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModulesRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModulesRegister.Implementation
{
    internal class CompilerModuleRegester : ICompilersModulesRegester
    {
        private readonly Dictionary<ExecutionStrategyType, ICompilerModule> _compilersModuels = new();

        public CompilerModuleRegester(IEnumerable<ICompilerModule> compilersModules)
        {
            foreach (var compilersModule in compilersModules)
            {
                if (_compilersModuels.ContainsKey(compilersModule.StategyType))
                {
                    throw new ArgumentException($"{compilersModule.StategyType} already definded compilerModule");
                }

                _compilersModuels.Add(compilersModule.StategyType, compilersModule);
            }
        }

        public bool TryResolveModule(ExecutionStrategyType executionStrategy, out ICompilerModule compilerModule)
        {
            return _compilersModuels.TryGetValue(executionStrategy, out compilerModule);
        }
    }
}
