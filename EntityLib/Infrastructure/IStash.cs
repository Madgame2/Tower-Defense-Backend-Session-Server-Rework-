using EntityLib.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EntityLib.Infrastructure
{
    internal interface IStash
    {
        int Size {  get; }

        ReadOnlySpan<int> EntityIds { get; }

        void Remove(Entity entity);
        bool Has(Entity entity);

    }
}
