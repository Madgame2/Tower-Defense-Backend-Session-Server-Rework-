using GameServer.Domain.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.PlayerStorages.Interfaces
{
    public interface IPlayersStorage
    {
        Player.Player? Get(string id);
        Player.Player[] GetAll();
        void Add(Player.Player player);
        void Remove(string id);
    }
}
