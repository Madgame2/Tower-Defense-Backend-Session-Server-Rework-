using GameServer.Application.Characters.Skadi;
using GameServer.Application.Components.GameObjectsComponents;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Enums;
using GameServer.Application.Components.GameObjectsComponents.HitBoxes.Meta.Model;
using GameServer.Application.Session.Services.GameObjectSpawner.Modules.Base;
using GameServer.Domain.Player;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.CharacterController;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Phisycs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Application.Session.Services.GameObjectSpawner.Modules
{
    public class PlayerGameObjectSpawnModule : IGameObjectSpawnModule<Player>
    {

        private readonly IServiceProvider serviceProvider;

        public PlayerGameObjectSpawnModule(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Configure(Player gameObject)
        {
            HitBoxComponent hitBox = CreateHitBoxComponent();
            CharacterController characterController = CreateCharacterController(gameObject);
            VelocityComponent velocityComponent = CreateVelocityComponent();

            gameObject.AddComponent(velocityComponent);
            gameObject.AddComponent(hitBox);
            gameObject.AddComponent(characterController);
        }

        private VelocityComponent CreateVelocityComponent()
        {
            return  new VelocityComponent();
        }

        private CharacterController CreateCharacterController(Player gameObject)
        {
            return  ActivatorUtilities.CreateInstance<SkadiController>(serviceProvider);
        }

        private HitBoxComponent CreateHitBoxComponent()
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
            return hitBox;
        }
    }
}
