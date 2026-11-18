using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Domain.Core.GameObjects;
using GameServer.Domain.Core.GameObjects.Components;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Base;
using GameServer.Domain.SessionWorld.Services.WorldQuery.SearchService.RegistryContext.Intrerfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using GameServer.SearchComponents.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Session.Services.ObjectRegister
{
    public class DefaultObjectRegister : ObjectRegistryContextBase
    {
        public override void Configure(TransitionRegistrat registrat)
        {
            registrat
                .Register<HitBoxComponent, GameObject>()
                .Get((in HitBoxComponent x, out GameObject result) => { result = x.GameObject; return true; });

            registrat
                .Register<GameObject, TransformComponent>()
                .Get((in GameObject x, out TransformComponent result) =>
                                               x.TryGetComponent(out result));

            registrat.Register<TransformComponent, PositionQueryData>()
                .Select(x => new PositionQueryData
                {
                    Position = x.Position
                });
        }
    }
}

