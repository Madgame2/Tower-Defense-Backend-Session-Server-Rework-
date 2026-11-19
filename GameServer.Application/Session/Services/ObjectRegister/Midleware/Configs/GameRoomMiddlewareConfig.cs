using GameServer.Application.Session.Services.ObjectRegister.Midleware.Modules;
using GameServer.Application.Session.Services.Repositories;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Builder;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.ObjectRegister.Midleware.Configs
{
    public static class GameRoomMiddlewareConfig
    {
        public static ObjectMiddlewareDescription Create() => new ObjectMiddlewareBuilder()
        .Use(s => new HitBoxInterceptor(s.Get<HitBoxRepository>()),
             o => o.OnlyOn(ObjectOperation.Add | ObjectOperation.Remove))
        .Build();
    }
}
