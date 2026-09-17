using EntityLib.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityLib.Models
{
    public sealed class Filter
    {
        public static FilterBuilder Create()
        {
            return new FilterBuilder();
        }

        internal readonly Type[] Include;
        internal readonly Type[] Exclude;

        internal Filter(Type[] include, Type[] exclude)
        {
            Include = include;
            Exclude = exclude;
        }
    }
}
