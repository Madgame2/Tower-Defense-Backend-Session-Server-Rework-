using EntityLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EntityLib.Services
{
    public sealed class FilterBuilder
    {
        private readonly List<Type> _include = new();
        private readonly List<Type> _exclude = new();

        public FilterBuilder With<T>()
            where T : struct, IComponent
        {
            _include.Add(typeof(T));
            return this;
        }

        public FilterBuilder Without<T>()
            where T : struct, IComponent
        {
            _exclude.Add(typeof(T));
            return this;
        }

        public Filter Create() =>
            new(
                _include.ToArray(),
                _exclude.ToArray());
    }
}
