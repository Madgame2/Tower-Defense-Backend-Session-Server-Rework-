using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.Interfaces;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Meta;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload;
using GameServer.Domain.SessionWorld.Interfaces.ReplicationService.Models.Spawn.Payload.PlayerSpawnEventModel;
using GameServer.Domain.SessionWorld.PlayerStorages.Interfaces;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using GameServer.GameObjectComponentsAndServices.GameObjectsComponents.Transform;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.ObjectRegister.Handlers
{
    public class PlayersRegisterHandler : IObjectHandler<Player.Player, uint>, IObjectHandler<Player.Player,string>, IReplicationModule<Player.Player>
    {
        public Type StoragedType => typeof(Player.Player);
        Type IReplicationModule.ProcessedType => typeof(Player.Player);

        private event Action<ServerEvent> _onReplication;

        event Action<ServerEvent> IReplicationModule.OnReplication
        {
            add => _onReplication += value;
            remove => _onReplication -= value;
        }
        private IPlayersStorage _playerStorage;


        public PlayersRegisterHandler(IPlayersStorage playersStorage)
        {
            _playerStorage = playersStorage;
        }

        public void Add(Player.Player newObject)
        {
            _playerStorage.Add(newObject);

            if(newObject.TryGetComponent<TransformComponent>(out var transform))
            {
                var payload = new SpawnPayload
                {
                    playerPayload = new PlayerSpawnPayload
                    {
                        Transforms = transform.GetShapShoot()
                    }
                };

                _onReplication?.Invoke(ServerEvent.Created(newObject.ObjectId, ObjectType.Player, payload));
            }
        }

        public void Change(Player.Player obj)
        {
            
        }

        public Player.Player? Get(uint arg)
        {
            return _playerStorage.Get(arg);
        }

        public void Remove(Player.Player obj)
        {
            _playerStorage.Remove(obj.PlayerId);
            _onReplication?.Invoke(ServerEvent.Removed());
        }

        public Player.Player? Get(string arg)
        {
            return _playerStorage.Get(arg);
        }
    }
}
