using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Domain.Services.StaticData.Meta
{
    public abstract class IncludedPrefabsRegistryBase : IEnumerable<string>
    {
        private readonly List<string> _filesPaths = new();

        public IEnumerator<string> GetEnumerator()
        {
            return _filesPaths.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void Include(string filePath)
        {
            if (_filesPaths.Contains(filePath))
                throw new ArgumentException($"prefab file {filePath} already defined");

            _filesPaths.Add(filePath);
        }
        public abstract void Incleded();
    }
}
