using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Model;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.Interfaces
{
    public interface IWorldQueryService : ILandscapeHeightService, IWorldSearchService, IPlayersRegisterService
    {

    }
}
