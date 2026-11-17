using GameServer.Domain.Services.StaticData.Meta;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Application.Resources.PrefabsRegesters
{
    public class GeneralPrefabsRegester : IncludedPrefabsRegistryBase
    {
        public override void Incleded()
        {
            Include("ObstaclesDescriptions/Tree_1.xml");
            Include("PrefabsRegesters/Characters/Skadi_character.xml");
            Include("PrefabsRegesters/Projectiles/Arrow_deffoult.xml");
        }
    }
}
