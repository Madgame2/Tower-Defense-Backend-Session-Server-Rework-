using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.DataBase.Models
{
    public struct TransitionModel
    {
        public Type From;
        public Type To;

        public Delegate transitionDelegate;
    }
}
