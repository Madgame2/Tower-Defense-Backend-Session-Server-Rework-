using GameServer.Domain.Common.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Ressources
{
    public class SessionStaticResources : ISessionStaticResources
    {

        private readonly IStaticDataService _staticDataService;

        public SessionStaticResources(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }
    }
}
