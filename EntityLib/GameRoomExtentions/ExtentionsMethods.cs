using EntityLib.Infrastructure;
using EntityLib.Models;
using GameServer.Domain.SessionWorld;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityLib.GameRoomExtentions
{
    public static class ExtentionsMethods
    {
        public static Entity CreateEntity(this GameRoom gameRoom)
        {

            var gameRoomData = ExtentinonStoredData.GetExternalData(gameRoom);
            var service = gameRoomData.EntityManager;

            var outEntity = service.CreateEntity();

            return outEntity;
        }

        public static void DeleteEntity(this GameRoom gameRoom, Entity entityForDeleating)
        {
            var gameRoomData = ExtentinonStoredData.GetExternalData(gameRoom);
            var service = gameRoomData.EntityManager;
            service.DestroyEntity(entityForDeleating);
        }

        public static Stash<T> GetStash<T>(this GameRoom gameRoom) where T: struct ,IComponent
        {
            var type = typeof(T);

            var gameRoomData = ExtentinonStoredData.GetExternalData(gameRoom);
            var stashes = gameRoomData.StashesStorage;

            if (stashes.TryGetValue(type, out var stash))
                return (Stash<T>)stash;

            var newStash = new Stash<T>();

            stashes.Add(type, newStash);

            return newStash;
        }


        public static EntityQuery GetEnities(this GameRoom gameRoom, Filter filter)

        {
            var data = ExtentinonStoredData.GetExternalData(gameRoom);
            var service = data.EntityManager;


            return data.EntityManager.SelectEntities(
                data.StashesStorage,
                filter);
        }
    }
}
