using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Delegates
{
    public delegate void ObjectDelegate<T>(ObjectContext<T> ctx);

}
