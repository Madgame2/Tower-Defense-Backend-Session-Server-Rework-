using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Application.Session.Services.ObjectRegister.Handlers;
using GameServer.Application.Session.Services.Repositories;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Context;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Delegates;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.ObjectRegister.Midleware.Modules
{
    public class HitBoxInterceptor : IObjectMiddleware
    {
        public HitBoxRepository _repository;

        public HitBoxInterceptor(HitBoxRepository repository)
        {
            _repository = repository;
        }

        public void Invoke<T>(ObjectContext<T> ctx, ObjectDelegate<T> next)
        {
            if (ctx.Object is GameObject gameObject)
            {
                if (gameObject.TryGetComponents<HitBoxComponent>(out var listOfHitBoxes))
                {
                    foreach (var hitBox in listOfHitBoxes)
                    {
                        _repository.Add(hitBox);
                    }
                }

                next(ctx);
            }
        }
    }
}
