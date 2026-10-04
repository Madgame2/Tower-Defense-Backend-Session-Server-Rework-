using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.Engine.Delegates
{
    public delegate void StepExecutorDelegate<T>(
        object state,
        QueryContext context,
        IList<T> buffer
    );
}
