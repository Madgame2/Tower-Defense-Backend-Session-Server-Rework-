using GameServer.Domain.Player;
using GameServer.Domain.SessionWorld.PlayerStorages.Interfaces;
using GameServer.Domain.SessionWorld.Services.ObjectRegister.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Services.ObjectRegister.Handlers
{
    public class PlayersRegisterHandler : IObjectHandler<Player.Player>
    {
        private IPlayersStorage _playerStorage;

        public PlayersRegisterHandler(IPlayersStorage playersStorage)
        {
            _playerStorage = playersStorage;
        }

        public void Add(Player.Player newObject)
        {
            _playerStorage.Add(newObject);
        }

        public void Change(Player.Player obj)
        {
            
        }

        public void Remove(Player.Player obj)
        {
            _playerStorage.Remove(obj.Id);
        }
    }
}
