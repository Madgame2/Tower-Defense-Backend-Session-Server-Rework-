using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.ChcekModulesRegister;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.ChcekModulesRegister.Interfaces;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules.Base;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.Implementation;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService
{
    public static class HitBoxCollisionServiceExtentions
    {
        public static IServiceCollection AddHitBoxServices(this IServiceCollection services)
        {
            services.AddSingleton<IHitBoxCollisionService, HitBoxCollisionService>();

            services.AddSingleton<ICollisionCheckStateryRegister, CollisionCheckStateryRegister>();

            services.AddSingleton<ShapesCompareModule, SphereVsSphere>();
            services.AddSingleton<ShapesCompareModule, CapsuleVsCapsule>();


            return services;
        }
    }
}
