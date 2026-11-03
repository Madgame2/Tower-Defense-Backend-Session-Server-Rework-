using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.ChcekModulesRegister.Interfaces;
using GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.CheckModules.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.GameObjectComponentsAndServices.Services.HitBoxesColitionService.ChcekModulesRegister
{
    public sealed class CollisionCheckStateryRegister : ICollisionCheckStateryRegister
    {
        private readonly Dictionary<
            (HitboxShapeType, HitboxShapeType),
            ShapesCompareModule> _modules = new();

        public CollisionCheckStateryRegister(
            IEnumerable<ShapesCompareModule> modules)
        {
            foreach (var module in modules)
            {
                _modules.Add(
                    (module.ShapeType1, module.ShapeType2),
                    module);

                if (module.ShapeType1 != module.ShapeType2)
                {
                    _modules.Add(
                        (module.ShapeType2, module.ShapeType1),
                        module);
                }
            }
        }

        public bool TryGetCheckModule(
            HitboxShapeType shape1,
            HitboxShapeType shape2,
            out CollisionCheckModule outModule)
        {
            if (_modules.TryGetValue((shape1, shape2), out var module))
            {
                outModule = module;
                return true;
            }

            outModule = null!;
            return false;
        }
    }
}
