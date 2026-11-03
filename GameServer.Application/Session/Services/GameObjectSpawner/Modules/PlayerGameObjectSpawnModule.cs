using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model;
using GameServer.Application.Session.Services.GameObjectSpawner.Modules.Base;
using GameServer.Domain.Player;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Session.Services.GameObjectSpawner.Modules
{
    public class PlayerGameObjectSpawnModule : IGameObjectSpawnModule<Player>
    {
        public void Configure(Player gameObject)
        {
            var capsuleShapedata = new CapsuleHitboxData();
            capsuleShapedata.Height = 2;
            capsuleShapedata.Radius = 0.5f;
            capsuleShapedata.Center = new Vector3(0, capsuleShapedata.Height / 2, 0);


            var shapeData = new HitboxData();
            shapeData.ShapeType = HitboxShapeType.Capsule;
            shapeData.Shape.Capsule = capsuleShapedata;

            var hitBox = new HitBoxComponent();
            hitBox.Data = shapeData;

            gameObject.AddComponent(hitBox);
        }
    }
}
