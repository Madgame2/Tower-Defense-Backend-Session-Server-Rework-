using GameServer.SearchComponents.Enums;
using GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModules.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.SearchComponents.ExecutionSteps.PlanComiler.CompilersModulesRegister.Interfaces
{
    public interface ICompilersModulesRegester
    {
        bool TryResolveModule(ExecutionStrategyType executionStrategy, out ICompilerModule compilerModule);
    }
}
