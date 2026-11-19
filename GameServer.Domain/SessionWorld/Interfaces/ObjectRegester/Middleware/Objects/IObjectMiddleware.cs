using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Context;
using GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Delegates;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.SessionWorld.Interfaces.ObjectRegester.Middleware.Objects
{
    public interface IObjectMiddleware
    {
        /// <summary>
        /// Код до next(ctx)  — выполняется ДО основного хендлера.
        /// Код после next(ctx) — выполняется ПОСЛЕ него.
        /// Не вызвал next — операция отменена (хендлер не будет вызван).
        /// </summary>
        void Invoke<T>(ObjectContext<T> ctx, ObjectDelegate<T> next);
    }
}
