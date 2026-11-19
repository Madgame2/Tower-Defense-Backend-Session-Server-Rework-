using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces
{
    public interface IObjectRegesterContext
    {
        TransitionModel[][] GetWay(Type from, Type to);
    }
}
